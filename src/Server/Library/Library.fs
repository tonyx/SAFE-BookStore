namespace SharpinoLibrary
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System
open Shared

module Library =
    type Library(wishLists: List<WishList> ) =
        let stateId = Guid.NewGuid()
        member this.WishLists =
            // strange thing to do but it is needed
            match box wishLists with
            | null -> []
            | _ -> wishLists

        member this.AddBook (username: UserName, book: Book) =
            let userWishList = this.WishLists |> List.tryFind (fun wl -> wl.UserName = username)
            let newWishList =
                match userWishList with
                | Some userWishList ->
                    let result = userWishList.VerifyNewBookIsNotADuplicate book
                    match result with
                    | Ok book ->
                        let newBooks = book :: userWishList.Books
                        let newWishList = { userWishList with Books = newBooks }
                        this.WishLists |> List.map (fun wl -> if wl.UserName = username then newWishList else wl)
                    | Error _ ->
                        this.WishLists
                | None ->
                    let newWishList = { UserName = username; Books = [book] }
                    newWishList :: this.WishLists
            Library (newWishList) |> Ok

        member this.RemoveBook (username: UserName, title: string) =
            let userWishList = this.WishLists |> List.tryFind (fun wl -> wl.UserName = username)
            let newWithList =
                match userWishList with
                | Some userWishList ->
                    let newBooks = userWishList.Books |> List.filter (fun b -> b.Title <> title)
                    let newWishList = { userWishList with Books = newBooks }
                    this.WishLists |> List.map (fun wl -> if wl.UserName = username then newWishList else wl)
                | None -> this.WishLists
            Library (newWithList) |> Ok

        member this.GetWishList (userName: UserName) =
            let wishList =
                this.WishLists
                |> List.tryFind (fun wl -> wl.UserName = userName)
                |> Option.map (fun wl -> wl.Books)
                |> Option.defaultValue []
            { UserName = userName; Books = wishList}

        member this.GetLastResetTime () =
            DateTime.UtcNow

    // ------------------- uniteresting members -------------------
        member this.StateId = stateId

        static member Zero =
            Library([])

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