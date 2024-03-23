module Server.Tests

open System // Add reference to the System assembly


open Expecto

// add server tests here
let all = testList "All" [ Shared.Tests.shared ]

    // Tests.runTestsInAssemblyWithCLIArgs ([]) argv

[<EntryPoint>]
let main _ = runTestsWithCLIArgs [] [||] all