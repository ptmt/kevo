module Kevo.Tests

open NUnit.Framework
open Kevo.Store
open System


let shouldBeTrue a = 
    Assert.AreEqual(a, true)


let shouldBeFalse a = 
    Assert.AreEqual(a, false)

let checkFilesForType<'t> =
    System.IO.Directory.GetFiles(Kevo.AppendLog.dataPath) |> Array.filter (fun x -> x.Contains(string typeof<'t>))

[<Test>]
let ``kevo should be able to append some integer`` () =
    checkFilesForType<int> |> Array.map (fun x -> System.IO.File.Delete x) |> ignore
    Kevo.Store.append<int> 1         
    checkFilesForType<int>.Length = 1 |> shouldBeTrue

[<Test>]
let ``kevo should be able to commit int`` () =
    Kevo.Store.append<int> 1             
    // check file
    let files = checkFilesForType<int>
    files.Length > 0 |> shouldBeTrue
    Kevo.AppendLog.commit<int> |> shouldBeTrue
    // file is must be deleted
    let files = checkFilesForType<int>
    files.Length = 0 |> shouldBeTrue
    // dictionary has one ore count
    

[<Test>]
let ``kevo should be correct process concurency commiting`` () =
    Kevo.Store.append<int> 1             
    async { [1..10] |> List.map (fun x -> Kevo.AppendLog.commit<int>) |> ignore } |> Async.RunSynchronously
    checkFilesForType<int>.Length = 0 |> shouldBeTrue
