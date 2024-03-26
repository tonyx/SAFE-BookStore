namespace SharpinoLibrary
open SharpinoLibrary.WishList
open SharpinoLibrary.WishListEvents
open Sharpino.Core
open Shared

module WishListCommands =
    type WishListCommands =
        | AddBook of Book
        | RemoveBook of string
            interface Command<WishListAggregate, WishListEvents> with
                member this.Execute (wishList: WishListAggregate) =
                    match this with
                    | AddBook book ->
                        wishList.AddBook book |> Result.map (fun _ -> [BookAdded book])
                    | RemoveBook title ->
                        wishList.RemoveBook title |> Result.map (fun _ -> [BookRemoved title])

                member this.Undoer = None