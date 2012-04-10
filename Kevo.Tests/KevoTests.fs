module Kevo.Tests

open NUnit.Framework
open Kevo.Store
open System


let shouldBeTrue a = 
    Assert.AreEqual(a, true)


let shouldBeFalse a = 
    Assert.AreEqual(a, false)


[<Test>]
let ``kevo should be able to append some integer`` () =
    Kevo.Store.append<int> 1         
    System.IO.File.Exists(Kevo.AppendLog.path DateTime.Now.Second) |> shouldBeTrue

[<Test>]
let ``kevo should be able to flushChanges`` () =
    Kevo.Store.append<int> 1             
    Kevo.AppendLog.flushChanges |> shouldBeTrue
    System.IO.File.Exists(Kevo.AppendLog.path DateTime.Now.Second) |> shouldBeFalse

[<Test>]
let ``kevo should be correct process concurency flushing`` () =
    Kevo.Store.append<int> 1             
    async { [1..4] |> List.map (fun x -> Kevo.AppendLog.flushChanges) |> ignore } |> Async.RunSynchronously
    System.IO.File.Exists(Kevo.AppendLog.path DateTime.Now.Second) |> shouldBeFalse
