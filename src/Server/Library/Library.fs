namespace SharpinoLibrary
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System
open Shared

module Library =
    type Library(books: Map<UserName, List<Book>>, wishLists: List<WishList> ) =

        let stateId = Guid.NewGuid()
        member this.StateId = stateId
        member this.Books = books
        member this.WishLists =
            // strange thing todo but it is needed
            match box wishLists with
            | null -> []
            | _ -> wishLists

        static member Zero =
            Library([] |> Map.ofList, []:> List<WishList>)

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

        member this.AddBook (username: UserName, book: Book) =
            let userWishList = this.WishLists |> List.tryFind (fun wl -> wl.UserName = username)
            let newWithList =
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
            Library (books, newWithList) |> Ok

        member this.RemoveBook (username: UserName, title: string) =
            let userWishList = this.WishLists |> List.tryFind (fun wl -> wl.UserName = username)
            let newWithList =
                match userWishList with
                | Some userWishList ->
                    let newBooks = userWishList.Books |> List.filter (fun b -> b.Title <> title)
                    let newWishList = { userWishList with Books = newBooks }
                    this.WishLists |> List.map (fun wl -> if wl.UserName = username then newWishList else wl)
                | None -> this.WishLists
            Library (books, newWithList) |> Ok

        member this.GetWishList (userName: UserName) =
            let wishList =
                this.WishLists
                |> List.tryFind (fun wl -> wl.UserName = userName)
                |> Option.map (fun wl -> wl.Books)
                |> Option.defaultValue []
            { UserName = userName; Books = wishList}

        member this.GetLastResetTime () =
            DateTime.UtcNow

