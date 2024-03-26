
namespace SharpinoLibrary
open SharpinoLibrary.Library
open SharpinoLibrary.LibraryEvents
open Sharpino.Core
open Shared
open System

module  LibraryCommands =
    type LibraryCommands =
        | AddUserRef of (string * Guid)
            interface Command<Library, LibraryEvents> with
                member this.Execute (library: Library) =
                    match this with
                    | AddUserRef (userName, id) ->
                        library.AddUserRef (userName, id) |> Result.map (fun _ -> [UserRefAdded (userName, id)])
                member this.Undoer = None
