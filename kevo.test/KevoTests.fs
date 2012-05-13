﻿module Kevo.Tests

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
    Kevo.Store.append<int>(1, 1, None)
    checkFilesForType<int>.Length = 1 |> shouldBeTrue

[<Test>]
let ``kevo should be able to commit int`` () =
    let dict = Kevo.Core.getDictionary<int>
    let length_before = dict.Count
    
    let random_value = System.Random().Next()
    Kevo.AppendLog.appendSync<int> length_before random_value     
    // just for for x.Contains(string DateTime.Now.Second) = false
    System.Threading.Thread.Sleep(1000)
    Kevo.AppendLog.commit<int> |> shouldBeTrue
    // file must be deleted
    let files = checkFilesForType<int>
    files.Length = 0 |> shouldBeTrue
    // dictionary must be with one more count
    Kevo.Core.getDictionary<int>.Count = 0 |> shouldBeFalse
    (Kevo.Core.getDictionary<int>.Count - length_before) = 1 |> shouldBeTrue
    Kevo.Core.getDictionary<int>.ContainsValue(random_value) |> shouldBeTrue
    // delete cache and try again
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<int>
    (Kevo.Core.getDictionary<int>.Count - length_before) = 1 |> shouldBeTrue
    Kevo.Core.getDictionary<int>.ContainsValue(random_value) |> shouldBeTrue

[<Test>]
let ``kevo should be correct process concurency commiting`` () =
    Kevo.Store.append<float32>(1, 1.0f, None)   
    System.Threading.Thread.Sleep(1000)        
    async { [1..10] |> List.map (fun x -> Kevo.AppendLog.commit<float32>) |> ignore } |> Async.RunSynchronously
    
    checkFilesForType<float32>.Length = 0 |> shouldBeTrue