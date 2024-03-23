module Storage

open System
// open Azure
// open Azure.Data.Tables
// open Azure.Storage.Blobs
open Shared
open System.Threading.Tasks

module Option =
    ()
    // let ofResponse (response: Response<'t>) =
    //     match response.HasValue with
    //     | true -> Some response.Value
    //     | false -> None

module BookTitle =
    ()
    // let isAllowed = string >> @"/\#?".Contains >> not

// type BookEntity() =
//     member val Title = "" with get, set
//     member val Authors = "" with get, set
//     member val ImageLink = "" with get, set
//     member val Link = "" with get, set

//     interface ITableEntity with
//         member val PartitionKey = "" with get, set
//         member val RowKey = "" with get, set
//         member val Timestamp = System.Nullable() with get, set
//         member val ETag = ETag.All with get, set

module BookEntity =
    ()
    // let buildEntity userName book =
    //     let entity: ITableEntity =
    //         BookEntity(Title = book.Title, Authors = book.Authors, Link = book.Link, ImageLink = book.ImageLink)

    //     entity.PartitionKey <- userName
    //     entity.RowKey <- book.Title.ToCharArray() |> Array.filter BookTitle.isAllowed |> String
    //     entity

module Defaults =
    let mockBooks = [
        {
            Title = "Get Programming with F#"
            Authors = "Isaac Abraham"
            ImageLink = "/images/Isaac.png"
            Link = "https://www.manning.com/books/get-programming-with-f-sharp"
        }

        {
            Title = "Mastering F#"
            Authors = "Alfonso Garcia-Caro Nunez"
            ImageLink = "/images/Alfonso.jpg"
            Link = "https://www.amazon.com/Mastering-F-Alfonso-Garcia-Caro-Nunez-ebook/dp/B01M112LR9"
        }

        {
            Title = "Stylish F#"
            Authors = "Kit Eason"
            ImageLink = "/images/Kit.jpg"
            Link = "https://www.apress.com/la/book/9781484239995"
        }
    ]

    let wishList = {
        UserName = UserName "test"
        Books = mockBooks
    }

// let getBooksTable (client: TableServiceClient) = task {
//     let table = client.GetTableClient "book"

//     // Azure will temporarily lock table names after deleting and can take some time before the table name is made available again.
//     let rec createTableSafe () = task {
//         try
//             do! table.CreateIfNotExistsAsync() :> Task
//         with _ ->
//             do! Task.Delay 5000
//             return! createTableSafe ()
//     }

//     do! createTableSafe ()
//     return table
// }

/// Load from the database
// let getWishListFromDB (client: TableServiceClient) (userName: UserName) = task {
//     let! table = getBooksTable client
//     let results = table.Query<BookEntity>($"PartitionKey eq '{userName.Value}'")

//     return {
//         UserName = userName
//         Books = [
//             for result in results ->
//                 {
//                     Title = result.Title
//                     Authors = result.Authors
//                     ImageLink = result.ImageLink
//                     Link = result.Link
//                 }
//         ]
//     }
// }

// let saveWishListToDB client wishList = task {
//     let! table = getBooksTable client

//     let existingBooks =
//         table.Query<BookEntity> $"PartitionKey eq '{wishList.UserName.Value}'"
//         |> Seq.toList

//     match existingBooks with
//     | [] -> ()
//     | existingBooks ->
//         printfn $"Deleting existing books... {existingBooks}"

//         do!
//             existingBooks
//             |> Seq.map (fun book -> TableTransactionAction(TableTransactionActionType.Delete, book))
//             |> table.SubmitTransactionAsync
//             :> Task

//     printfn $"Inserting books {wishList.Books}"

//     do!
//         wishList.Books
//         |> Seq.map (fun book ->
//             let tableEntity = BookEntity.buildEntity wishList.UserName.Value book
//             TableTransactionAction(TableTransactionActionType.UpsertReplace, tableEntity))
//         |> table.SubmitTransactionAsync
//         :> Task

//     ()
// }

module StateManagement =
    ()
    // let getStateBlob (client: BlobServiceClient) name = task {
    //     let state = client.GetBlobContainerClient "state"
    //     do! state.CreateIfNotExistsAsync() :> Task
    //     return state.GetBlobClient name
    // }

    // let resetTimeBlob client = getStateBlob client "resetTime"

    // let storeResetTime client = task {
    //     let! blob = resetTimeBlob client
    //     do! blob.DeleteIfExistsAsync() :> Task
    //     do! BinaryData "" |> blob.UploadAsync :> Task
    // }

// let getLastResetTime client systemStartTime = task {
//     let! blob = StateManagement.resetTimeBlob client

//     try
//         let! response = blob.GetPropertiesAsync()

//         return
//             response
//             |> Option.ofResponse
//             |> Option.map _.LastModified.UtcDateTime
//             |> Option.defaultValue systemStartTime

//     with :? RequestFailedException ->
//         return systemStartTime
// }