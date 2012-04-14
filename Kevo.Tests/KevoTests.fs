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
    let dict = Kevo.Core.getDictionary<int>
    let length_before = dict.Count

    Kevo.Store.append<int> 1      
   // Kevo.Core.getDictionary<int>.Count - length_before = 1 |> shouldBeTrue
    // check file
 //   let files = checkFilesForType<int>
 //   files.Length > 0 |> shouldBeTrue
    Kevo.AppendLog.commit<int> |> shouldBeTrue
    // file must be deleted
    let files = checkFilesForType<int>
    files.Length = 0 |> shouldBeTrue
    // dictionary has one more count
    Kevo.Core.getDictionary<int>.Count - length_before = 1 |> shouldBeTrue
    // delete cache and try again
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<int>
    Kevo.Core.getDictionary<int>.Count - length_before = 1 |> shouldBeTrue

[<Test>]
let ``kevo should be correct process concurency commiting`` () =
    Kevo.Store.append<int> 1             
    async { [1..10] |> List.map (fun x -> Kevo.AppendLog.commit<int>) |> ignore } |> Async.RunSynchronously
    checkFilesForType<int>.Length = 0 |> shouldBeTrue
