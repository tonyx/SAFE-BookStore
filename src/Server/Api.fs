module Api

// open Azure.Data.Tables
// open Azure.Storage.Blobs
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.AspNetCore.Http
open SAFE
open Shared
open Storage
open System
open SharpinoVersionApi.SharpinoApi
open Sharpino.Storage
open Sharpino
open Sharpino.MemoryStorage

let systemStartTime = DateTime.UtcNow

[<AutoOpen>]
module private Implementation =
    ()
    // let addBook tableStorage (username: UserName, book) = task {
    //     let! table = getBooksTable tableStorage
    //     let entity = BookEntity.buildEntity username.Value book
    //     do! table.AddEntityAsync entity |> Async.AwaitTask |> Async.Ignore

    //     return {
    //         Title = book.Title
    //         Authors = book.Authors
    //         ImageLink = book.ImageLink
    //         Link = book.Link
    //     }
    // }

    // let removeBook tableStorage (username: UserName, title: string) = task {
    //     let! table = getBooksTable tableStorage
    //     let partitionKey = username.Value
    //     let rowKey = title.ToCharArray() |> Array.filter BookTitle.isAllowed |> String
    //     do! table.DeleteEntityAsync(partitionKey, rowKey) |> Async.AwaitTask |> Async.Ignore
    //     return title
    // }

let create api =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext api
    |> Remoting.withErrorHandler ErrorHandling.errorHandler
    |> Remoting.buildHttpHandler

// let wishlistApi (context: HttpContext) =
//     let tableStorage = context.GetService<TableServiceClient>()
//     let blobStorage = context.GetService<BlobServiceClient>()

//     {
//         getWishlist = getWishListFromDB tableStorage >> Async.AwaitTask
//         addBook = addBook tableStorage >> Async.AwaitTask
//         removeBook = removeBook tableStorage >> Async.AwaitTask
//         getLastResetTime = fun () -> getLastResetTime blobStorage systemStartTime |> Async.AwaitTask
//     }

let memoryStorage: IEventStore = MemoryStorage()
let sharpinoWishListApi = LibraryApi(memoryStorage)

let sharpinoWishlistApi (context: HttpContext) =
    {
        getWishlist = fun username ->
            async {
                let wishList = sharpinoWishListApi.GetWishList username
                match wishList with
                | Ok wishList -> return wishList
                |  _ -> return { UserName = username; Books = [] }
            }
        addBook = fun (username, book) ->
            async {
                match sharpinoWishListApi.AddBook (username, book)with
                | Ok _ -> return book
                | _ -> return book
            }

        removeBook = fun (userName, title) -> async {
            match sharpinoWishListApi.RemoveBook (userName, title) with
            | Ok _ -> return title
            | _ -> return title
        }
        getLastResetTime = fun () ->
            async {
                match sharpinoWishListApi.GetLastResetTime() with
                | Ok time -> return time
                | _ -> return failwith "error getting last reset time"
            }
    }

let guestApi ctx = {
    getBooks = fun () -> async { return Defaults.mockBooks }
    login = fun user -> async { return Authorise.login user }
}