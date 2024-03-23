namespace SharpinoLibrary
open SharpinoLibrary.Library
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino.Core
open Shared
open System

module LibraryEvents =
    type LibraryEvents =

        | BookAdded of UserName * Book
        | BookRemoved of UserName * string
            interface Event<Library> with
                member this.Process (library: Library) =
                    match this with
                    | BookAdded (userName, book) -> library.AddBook (userName, book)
                    | BookRemoved (username, title) -> library.RemoveBook (username, title)

        static member Deserialize (serializer: ISerializer, json: Json): Result<LibraryEvents, string> =
            serializer.Deserialize<LibraryEvents> json
        member this.Serialize (serializer: ISerializer) =
            this
            |> serializer.Serialize


