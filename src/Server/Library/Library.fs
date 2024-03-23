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

        static member Zero =
            Library([] |> Map.ofList, [])

        static member StorageName =
            "_library"

        static member Version =
            "_01"

        static member SnapshotsInterval =
            10

        static member Lock =
            new Object()

        static member Deserialize (serializer: ISerializer, json: Json): Result<Library, string>  =
            serializer.Deserialize<Library> json

        member this.Serialize (serializer: ISerializer) =
            this
            |> serializer.Serialize

        member this.AddBook (username: UserName, book: Book) =
            let newBooks =
                match this.Books.TryFind username with
                | Some books -> book :: books
                | None -> [book]
            let newBooksMap = this.Books.Add(username, newBooks)
            Library (newBooksMap, wishLists) |> Ok

        member this.RemoveBook (username: UserName, title: string) =
            let newBooks =
                match this.Books.TryFind username with
                | Some books -> books |> List.filter (fun b -> b.Title <> title)
                | None -> []
            let newBooksMap = this.Books.Add(username, newBooks)
            Library (newBooksMap, wishLists) |> Ok

        member this.GetWishList (userName: UserName) =
            let wishList =
                wishLists
                |> List.tryFind (fun wl -> wl.UserName = userName)
                |> Option.map (fun wl -> wl.Books)
                |> Option.defaultValue []
            { UserName = userName; Books = wishList}

        member this.GetLastResetTime () =
            DateTime.UtcNow

