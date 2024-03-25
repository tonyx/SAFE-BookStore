module Api

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.AspNetCore.Http
open SAFE
open Shared
open Storage
open System
open SharpinoVersionApi.SharpinoApi
open Sharpino.Storage
open Sharpino.MemoryStorage
open Sharpino.PgStorage

let systemStartTime = DateTime.UtcNow

[<AutoOpen>]
module private Implementation =
    ()

let create api =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext api
    |> Remoting.withErrorHandler ErrorHandling.errorHandler
    |> Remoting.buildHttpHandler


let memoryStorage: IEventStore = MemoryStorage()
let dbStorage: IEventStore = PgEventStore(SharpinoVersionApi.SharpinoApi.connection)
// let sharpinoApi = LibraryApi(memoryStorage)
let sharpinoApi = LibraryApi(dbStorage)

let sharpinoWishlistApi (context: HttpContext) =
    {
        getWishlist = fun username ->
            async {
                let wishList = sharpinoApi.GetWishList username
                match wishList with
                | Ok wishList -> return wishList
                |  _ -> return { UserName = username; Books = [] }
            }
        addBook = fun (username, book) ->
            async {
                match sharpinoApi.AddBook (username, book)with
                | Ok _ -> return book
                | _ -> return book
            }

        removeBook = fun (userName, title) -> async {
            match sharpinoApi.RemoveBook (userName, title) with
            | Ok _ -> return title
            | _ -> return title
        }
        getLastResetTime = fun () ->
            async {
                match sharpinoApi.GetLastResetTime() with
                | Ok time -> return time
                | _ -> return failwith "error getting last reset time"
            }
    }

let guestApi ctx = {
    getBooks = fun () -> async { return Defaults.mockBooks }
    login = fun user -> async { return Authorise.login user }
}