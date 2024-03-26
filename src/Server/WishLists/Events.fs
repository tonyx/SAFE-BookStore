
namespace SharpinoLibrary
open SharpinoLibrary.Library
open SharpinoLibrary.WishList
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino.Core
open Shared
open System
open MBrace.FsPickler.Json

module WishListEvents =
    let pickler = FsPickler.CreateJsonSerializer(indent = false)
    type WishListEvents =
        | BookAdded of Book
        | BookRemoved of string
            interface Event<WishListAggregate> with
                member this.Process (wishList: WishListAggregate) =
                    match this with
                    | BookAdded book -> wishList.AddBook book
                    | BookRemoved title -> wishList.RemoveBook title

        static member Deserialize (serializer: ISerializer, json: Json): Result<WishListEvents, string> =
            let result = pickler.UnPickleOfString json
            printf "withlistevent pickler result %A\n" result
            result |> Ok
            // Error "asdf"
            // result
            // serializer.Deserialize<WishListEvents> json
        member this.Serialize (serializer: ISerializer) =
            pickler.PickleToString this
            // this
            // |> serializer.Serialize
