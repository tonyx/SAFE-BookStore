namespace SharpinoLibrary
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System
open Shared

module Library =
    type Library(wishListAggregatesRefs: Map<UserName, Guid> ) =
        let stateId = Guid.NewGuid()

        member this.WishListAggregatesRefs =
            // strange thing to do but it is needed
            match box wishListAggregatesRefs with
            | null -> Map.empty
            | _ -> wishListAggregatesRefs

        member this.AddUserRef (userName: UserName, whishListAggregateId: Guid) =
            // let newRef = Guid.NewGuid()
            ResultCE.result
                {
                    let userExists = this.WishListAggregatesRefs.ContainsKey userName
                    let! shouldNotExist =
                        userExists
                        |> not
                        |> Result.ofBool "user already exists"
                    let newRefs = this.WishListAggregatesRefs.Add(userName, whishListAggregateId)
                    return Library(newRefs)
                }

        member this.TryGetUserWishListAggregateId (userName: UserName) =
            let userRef = this.WishListAggregatesRefs.TryFind userName
            userRef
        member this.GetUserWishlListAggregateId (userName: UserName) =
            ResultCE.result {
                let userRef = this.WishListAggregatesRefs.TryFind userName
                match userRef with
                | Some userRef ->
                    return userRef
                | None ->
                    return! "no user found" |> Error
            }

        member this.GetLastResetTime () =
            DateTime.UtcNow

    // ------------------- uniteresting members -------------------
        member this.StateId = stateId

        static member Zero =
            Library(Map.empty)

        static member StorageName =
            "_library"

        static member Version =
            "_01"

        static member SnapshotsInterval =
            3

        static member Lock =
            new Object()
        static member Deserialize (serializer: ISerializer, json: Json): Result<Library, string>  =
            serializer.Deserialize<Library> json

        member this.Serialize (serializer: ISerializer) =
            this
            |> serializer.Serialize