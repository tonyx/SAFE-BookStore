module Storage

open System
open Shared
open System.Threading.Tasks

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

