module Kevo.Tests

open NUnit.Framework
open Kevo.Store
open System
open ProtoBuf


let shouldBeTrue a = 
    Assert.AreEqual(a, true)


let shouldBeFalse a = 
    Assert.AreEqual(a, false)

let checkFilesForType<'t> =
    System.IO.Directory.GetFiles(Kevo.AppendLog.dataPath) |> Array.filter (fun x -> x.Contains(string typeof<'t>))

[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
type rr (find : string, replace : string) = class  
    member val Find : string = find with get, set   
    member val Replace : string = replace with get, set   
    new() = rr("", "")
    override x.ToString() = x.Find
 end  

let rec waitForIt<'t> timeout currenttime = 
    if currenttime < timeout && (checkFilesForType<'t>).Length > 0 then
        System.Threading.Thread.Sleep(100)  
        Kevo.AppendLog.commit<'t> |> ignore
        waitForIt<'t> timeout (currenttime + 100)
    else
        (checkFilesForType<'t>).Length > 0
        

[<Test>]
let ``kevo should be able to append some integer`` () =
    checkFilesForType<int> |> Array.map (fun x -> System.IO.File.Delete x) |> ignore
    Kevo.Store.append<int>(1, 1, Some (fun () -> System.Threading.Thread.Sleep(1000) ))
    checkFilesForType<int>.Length = 1 |> shouldBeTrue
    waitForIt<int> 5000 0 |> shouldBeFalse

[<Test>]
let ``kevo should be able to append to log some difficult type`` () =
    checkFilesForType<rr> |> Array.map (fun x -> System.IO.File.Delete x) |> ignore
    Kevo.Store.append<rr>(1, new rr("aa", "adfa"), None)
    checkFilesForType<rr>.Length = 1 |> shouldBeTrue

[<Test>]
let ``kevo should be able to commit int into memory and flush to disk`` () =
    checkFilesForType<int> |> Array.map (fun x -> System.IO.File.Delete x) |> ignore
    let dict = Kevo.Core.getDictionary<int>
    let length_before = dict.Count + 1
    
    let random_value = System.Random(100).Next()
    Kevo.AppendLog.appendSync<int> length_before random_value     
  
    //Kevo.AppendLog.commit<int> |> shouldBeTrue   
    
    // all files must be deleted
    waitForIt<int> 5000 0 |> shouldBeFalse
  
    // dictionary must be with one more count
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<int>
    let dict = Kevo.Core.getDictionary<int>
    dict.Count = 0 |> shouldBeFalse
    //(dict.Count - length_before) = 1 |> shouldBeTrue
    dict.ContainsValue(random_value) |> shouldBeTrue
  

[<Test>]
let ``kevo should be correct process concurency commiting`` () =
    Kevo.AppendLog.appendSync<double> 1 1.0
    //System.Threading.Thread.Sleep(1000)        
    async { [1..10] |> List.map (fun x -> Kevo.AppendLog.commit<double>) |> ignore } |> Async.RunSynchronously
    waitForIt<double> 5000 0 |> shouldBeFalse
    checkFilesForType<double>.Length = 0 |> shouldBeTrue

[<Test>]
let ``kevo should be able to update element`` () =
    // clear
    checkFilesForType<float32> |> Array.map (fun x -> System.IO.File.Delete x) |> ignore
    checkFilesForType<float32>.Length = 0 |> shouldBeTrue    
    // add first
    Kevo.Store.update<float32>(1, 1.0f)  
    waitForIt<float32> 5000 0 |> shouldBeFalse
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<float32>
    Kevo.Core.getDictionary<float32>.[1] = 1.0f |> shouldBeTrue  
    // update
    Kevo.Store.update<float32>(1, 2.0f)  
    waitForIt<float32> 5000 0 |> shouldBeFalse
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<float32>
    Kevo.Core.getDictionary<float32>.[1] = 2.0f |> shouldBeTrue



[<Test>]
let ``kevo should be able clear cache`` () =
    let dict = Kevo.Core.getDictionary<int>
    dict = null |> shouldBeFalse
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<int>
    Kevo.MemoryCache.checkCache Kevo.Core.cacheIndex<int> |> shouldBeFalse
    

[<Test>]
let ``kevo should be able to delete item`` () =
    checkFilesForType<int> |> Array.map (fun x -> System.IO.File.Delete x) |> ignore
//    Kevo.Store.append<int>(1, 1, None)
    //waitForIt<int> 5000 0 |> shouldBeFalse
    Kevo.Store.delete<int> 1
//    Kevo.AppendLog.commit<int> |> shouldBeTrue
    waitForIt<int> 5000 0 |> shouldBeFalse
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<int>
    Kevo.Core.getDictionary<int>.ContainsKey(1) |> shouldBeFalse