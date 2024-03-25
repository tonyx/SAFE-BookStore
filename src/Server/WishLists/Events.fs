
namespace SharpinoLibrary
open SharpinoLibrary.Library
open SharpinoLibrary.WishList
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino.Core
open Shared
open System

module WishListEvents =
    type WishListEvents =
        | BookAdded of Book
        | BookRemoved of string
            interface Event<AggregateWishList> with
                member this.Process (wishList: AggregateWishList) =
                    match this with
                    | BookAdded book -> wishList.AddBook book
                    | BookRemoved title -> wishList.RemoveBook title

        static member Deserialize (serializer: ISerializer, json: Json): Result<WishListEvents, string> =
            serializer.Deserialize<WishListEvents> json
        member this.Serialize (serializer: ISerializer) =
            this
            |> serializer.Serialize
