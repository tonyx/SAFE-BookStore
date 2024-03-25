namespace SharpinoLibrary
open Shared
open Sharpino.CommandHandler
open Sharpino.Definitions
open Sharpino
open Sharpino.Storage
open Sharpino.Core
open Sharpino.Utils
open Sharpino.Lib.Core.Commons
open FSharpPlus.Operators
open FsToolkit.ErrorHandling
open System

module WishList =
    type AggregateWishList(id: Guid, wishList: WishList) =
        let stateId = Guid.NewGuid()
        member this.WishList = wishList
        member this.Id = id
        member this.StateId = stateId

        member this.AddBook (book: Book) =
            ResultCE.result {
                let! book = this.WishList.VerifyNewBookIsNotADuplicate book
                let newWishList = { this.WishList with Books = book :: this.WishList.Books }
                return AggregateWishList(this.Id, newWishList)
            }
        member this.RemoveBook (title: string) =
            ResultCE.result {
                let! bookExists =
                    this.WishList.Books |> List.tryFind (fun b -> b.Title = title)
                    |> Result.ofOption "book not found"
                let newBooks = this.WishList.Books |> List.filter (fun b -> b.Title <> title)
                let newWishList = { this.WishList with Books = newBooks }
                return AggregateWishList(this.Id, newWishList)
            }
        member this.GetWishList () =
            this.WishList

    // ------------------- uniteresting members -------------------

        static member StorageName =
            "_wishlist"
        static member Version =
            "_01"
        static member SnapshotsInterval =  15
        member this.Serialize (serializer: ISerializer) =
            this
            |> serializer.Serialize
        static member Deserialize (serializer: ISerializer, json: Json): Result<AggregateWishList, string>  =
            serializer.Deserialize<AggregateWishList> json

        interface Aggregate with
            member this.StateId = stateId
            member this.Id = this.Id
            member this.Lock: obj =
                this
            member this.Serialize(serializer: ISerializer): string =
                this.Serialize serializer

        interface Entity with
            member this.Id = this.Id
