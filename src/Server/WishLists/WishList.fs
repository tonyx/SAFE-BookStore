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
open MBrace.FsPickler.Json
open System

module WishList =
    let pickler = FsPickler.CreateJsonSerializer(indent = false)

    type WishListAggregate(id: Guid, wishList: WishList) =
        let stateId = Guid.NewGuid()
        member this.WishList = wishList
        member this.Id = id
        member this.StateId = stateId

        member this.AddBook (book: Book) =
            printf "adding book\n"
            ResultCE.result {
                let! book = this.WishList.VerifyNewBookIsNotADuplicate book
                let newWishList = { this.WishList with Books = book :: this.WishList.Books }
                return WishListAggregate(this.Id, newWishList)
            }
        member this.RemoveBook (title: string) =
            printf "removing book\n"
            ResultCE.result {
                let! bookExists =
                    this.WishList.Books |> List.tryFind (fun b -> b.Title = title)
                    |> Result.ofOption "book not found"
                let newBooks = this.WishList.Books |> List.filter (fun b -> b.Title <> title)
                let newWishList = { this.WishList with Books = newBooks }
                return WishListAggregate(this.Id, newWishList)
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
            pickler.PickleToString this
            // this
            // |> serializer.Serialize
        // member this.Serialize (serializer: ISerializer) =
        //     this
        //     |> serializer.Serialize
        // static member Deserialize (serializer: ISerializer, json: Json): Result<WishListAggregate, string>  =
        //     serializer.Deserialize<WishListAggregate> json
        static member Deserialize (serializer: ISerializer, json: Json): Result<WishListAggregate, string>  =
            let result = pickler.UnPickleOfString json
            printf "deserialized %A\n" result
            result |> Ok
            // pickler.UnPickleFromString<WishListAggregate> json

        interface Aggregate with
            member this.StateId = stateId
            member this.Id = this.Id
            member this.Lock: obj =
                this
            member this.Serialize(serializer: ISerializer): string =
                this.Serialize serializer

        interface Entity with
            member this.Id = this.Id
