namespace SharpinoLibrary
open SharpinoLibrary.Library
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino.Core
open Shared
open System

module LibraryEvents =
    type LibraryEvents =

        | UserRefAdded of UserName * Guid
            interface Event<Library> with
                member this.Process (library: Library) =
                    match this with
                    | UserRefAdded (userName, id) -> library.AddUserRef (userName, id)

        static member Deserialize (serializer: ISerializer, json: Json): Result<LibraryEvents, string> =
            serializer.Deserialize<LibraryEvents> json
        member this.Serialize (serializer: ISerializer) =
            this
            |> serializer.Serialize


