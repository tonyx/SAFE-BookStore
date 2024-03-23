
namespace SharpinoLibrary
open SharpinoLibrary.Library
open SharpinoLibrary.LibraryEvents
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino.Core
open Shared
open System

module  LibraryCommands =
    type LibraryCommands =
        | AddBook of UserName * Book
        | RemoveBook of UserName* string
            interface Command<Library, LibraryEvents> with
                member this.Execute (library: Library) =
                    match this with
                    | AddBook (userName, book) ->
                        library.AddBook (userName, book) |> Result.map (fun _ -> [BookAdded (userName, book)])
                    | RemoveBook (userName, title) ->
                        library.RemoveBook (userName, title) |> Result.map (fun _ -> [BookRemoved (userName, title)])
                member this.Undoer = None
