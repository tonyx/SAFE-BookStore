namespace SharpinoLibrary
open SharpinoLibrary.Library
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino.Core
open Shared
open System
open MBrace.FsPickler.Json

module LibraryEvents =
    let pickler = FsPickler.CreateJsonSerializer(indent = false)
    type LibraryEvents =

        | UserRefAdded of string * Guid
            interface Event<Library> with
                member this.Process (library: Library) =
                    match this with
                    | UserRefAdded (userName, id) -> library.AddUserRef (userName, id)

        static member Deserialize (serializer: ISerializer, json: Json): Result<LibraryEvents, string> =
            let result = pickler.UnPickleOfString json
            printf "library events. pickler result %A\n" result
            result |> Ok

            // serializer.Deserialize<LibraryEvents> json
        member this.Serialize (serializer: ISerializer) =
            pickler.PickleToString this
            // this
            // |> serializer.Serialize


