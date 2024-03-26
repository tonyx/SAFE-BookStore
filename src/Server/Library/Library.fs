namespace SharpinoLibrary
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System
open Shared
open MBrace.FsPickler.Json

module Library =
    let pickler = FsPickler.CreateJsonSerializer(indent = false)
    type Library(wishListAggregatesRefs: Map<string, Guid> ) =
        let stateId = Guid.NewGuid()

        member this.WishListAggregatesRefs =
            // strange thing to do but it is needed
            match box wishListAggregatesRefs with
            | null -> Map.empty
            | _ -> wishListAggregatesRefs

        member this.AddUserRef (userName: string, whishListAggregateId: Guid) =
            ResultCE.result
                {
                    let userExists = this.WishListAggregatesRefs.ContainsKey userName
                    let! shouldNotExist =
                        userExists
                        |> not
                        |> Result.ofBool "user already exists"
                    printf "adding user %A\n" userName
                    let newRefs = this.WishListAggregatesRefs.Add(userName, whishListAggregateId)
                    return Library(newRefs)
                }

        member this.TryGetUserWishListAggregateId (userName: string) =
            let userRef = this.WishListAggregatesRefs.TryFind userName
            userRef
        member this.GetUserWishlListAggregateId (userName: string) =
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
        // static member Deserialize (serializer: ISerializer, json: Json): Result<Library, string>  =
        //     serializer.Deserialize<Library> json
        static member Deserialize (serializer: ISerializer, json: Json): Result<Library, string>  =
            pickler.UnPickleOfString json
            // serializer.Deserialize<Library> json

        member this.Serialize (serializer: ISerializer) =
            pickler.PickleToString this
        // member this.Serialize (serializer: ISerializer) =
        //     this
        //     |> serializer.Serialize