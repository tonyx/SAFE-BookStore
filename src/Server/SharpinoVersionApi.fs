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
open Sharpino.CommandHandler
open Sharpino
open System

module SharpinoApi =
    open Shared
    let connection =
        "Server=127.0.0.1;"+
        "Database=es_pub_sharpino;" +
        "User Id=safe;"+
        "Password=safe;"

    type LibraryApi (eventStore: IEventStore) =
        let libraryStateViewer = getStorageFreshStateViewer<Library, LibraryEvents> eventStore
        let doNothingBroker: IEventBroker =
            {
                notify = None
                notifyAggregate = None
            }

        member this.AddBook (userName: UserName, book: Book) =
            ResultCE.result {
                let command = AddBook (userName, book)
                let! result = runCommand<Library, LibraryEvents> eventStore doNothingBroker libraryStateViewer command
                return result
            }

        member this.RemoveBook (userName: UserName, title: string) =
            ResultCE.result {
                let command = RemoveBook (userName, title)
                let! result = runCommand<Library, LibraryEvents> eventStore doNothingBroker libraryStateViewer command
                return result
            }

        member this.GetWishList (userName: UserName) =
            ResultCE.result {
                let! (_, state, _, _) = libraryStateViewer()
                let wishList =
                    state.GetWishList userName
                return wishList
            }
        member this.GetLastResetTime() =
            ResultCE.result {
                let! (_, state, _, _) = libraryStateViewer()
                let lastResetTime =
                    state.GetLastResetTime()
                return lastResetTime
            }
