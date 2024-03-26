namespace SharpinoVersionApi
open Sharpino
open SharpinoLibrary.Library
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Storage
open Sharpino.Utils
open SharpinoLibrary.LibraryEvents
open SharpinoLibrary.LibraryCommands
open SharpinoLibrary.WishList
open SharpinoLibrary.WishListCommands
open SharpinoLibrary.WishListEvents
open Sharpino.CommandHandler
open Sharpino
open System

module SharpinoApi =
    open Shared
    let connection =
        "Server=127.0.0.1;"+
        "Database=safe_sharpino_lib;" +
        "User Id=safe;"+
        "Password=safe;"

    type LibraryApi (eventStore: IEventStore) =
        let libraryStateViewer = getStorageFreshStateViewer<Library, LibraryEvents> eventStore
        let wishListStateViewer = getAggregateStorageFreshStateViewer<WishListAggregate, WishListEvents> eventStore

        let doNothingBroker: IEventBroker =
            {
                notify = None
                notifyAggregate = None
            }

        member this.InitUser (userName: UserName, wishListId: Guid) =
            ResultCE.result {
                let command = AddUserRef (userName.Value, wishListId)
                let aggregateWishList = WishListAggregate(wishListId, { UserName = userName; Books = [] })
                let! result = runInitAndCommand<Library, LibraryEvents, WishListAggregate> eventStore doNothingBroker libraryStateViewer aggregateWishList command
                return result
            }
        member this.UserNotExists (userName: UserName) =
            ResultCE.result {
                let! (_, state, _, _) = libraryStateViewer()
                let userNotExists =
                    state.TryGetUserWishListAggregateId userName.Value
                    |> Option.isNone
                return userNotExists
            }
        member this.MakeUserIfNotExists (userName: UserName) =
            ResultCE.result {
                let! userNotExists =
                    this.UserNotExists userName
                if userNotExists then
                    let wishListId = Guid.NewGuid()
                    let! result = this.InitUser (userName, wishListId)
                    return ()
                else
                    return ()
            }
        member this.AddBook (userName: UserName, book: Book) =
            ResultCE.result {
                let _ = this.MakeUserIfNotExists userName
                let! (_, libraryState, _, _) = libraryStateViewer()
                let! wishList = libraryState.GetUserWishlListAggregateId userName.Value
                let command = WishListCommands.AddBook book
                let! result = runAggregateCommand<WishListAggregate, WishListEvents> wishList eventStore doNothingBroker wishListStateViewer command
                return result
            }

        member this.RemoveBook (userName: UserName, title: string) =
            ResultCE.result {
                let! (_, libraryState, _, _) = libraryStateViewer()
                let! wishList = libraryState.GetUserWishlListAggregateId userName.Value
                let command = WishListCommands.RemoveBook title
                let! result = runAggregateCommand<WishListAggregate, WishListEvents> wishList eventStore doNothingBroker wishListStateViewer command
                return result
            }

        member this.GetWishList (userName: UserName) =
            ResultCE.result {
                let! (_, libraryState, _, _) = libraryStateViewer()
                let! wishListId = libraryState.GetUserWishlListAggregateId userName.Value
                let! (_, wishListState, _, _) = wishListStateViewer wishListId
                let wishList = wishListState.GetWishList()
                return wishListState.GetWishList()
            }

        member this.GetLastResetTime() =
            ResultCE.result {
                let! (_, state, _, _) = libraryStateViewer()
                let lastResetTime =
                    state.GetLastResetTime()
                return lastResetTime
            }
