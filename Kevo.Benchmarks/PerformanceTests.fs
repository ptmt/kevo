﻿module Kevo.PerformanceTests

open System.Diagnostics
open Kevo.Store
open Kevo.JsonNet
open System.Collections.Generic
open ProtoBuf

let duration name f = 
    let timer = new System.Diagnostics.Stopwatch()
    timer.Start()
    let returnValue = f()
    printfn "%A Ellapsed Time: %f ms" name timer.Elapsed.TotalMilliseconds
    returnValue 


let inline testReadSumAllNones<'t> count =
    [ 1 .. count] |> List.filter (fun x-> (findById<'t> x).IsNone) |> List.fold (+) 0

let inline testStraightWildcardSearch<'t> query = 
    Kevo.Store.findByQuery<'t> query |> printfn "%A"

//let inline testStraightWildcardSearchSyn<'t> query = 
    //Kevo.Store.findByQuery<'t> query |> List.collect (fun x -> x.Syn) 
let inline testDeserializeFromProtoBuf<'t> =
    let c = Kevo.Core.getDictionary<'t>
    //let k = Kevo.ProtoBuf.deserialize<'t>
    Kevo.ProtoBuf.serialize<Dictionary<int, 't>> c (string typeof<'t>.GUID)
    Kevo.JsonNet.serialize<Dictionary<int, 't>> c
    let b = Kevo.JsonNet.deserialize<Dictionary<int, 't>>    
    b


let inline testAppend c =   
   let timer = new System.Diagnostics.Stopwatch()
   timer.Start()
   [0..c] |> List.map (fun x -> Kevo.Store.append<string>(x, (string x), None))  |> ignore
   let perf = float c / timer.Elapsed.TotalMilliseconds
   printfn "%i; %f" c perf
   Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<string>
   printfn "%A" Kevo.Core.getDictionary<string>.Count

   
let testAppendLoop lmax step= 
    [0..step..lmax] |> List.map (fun x -> testAppend x) |> ignore    
    System.Threading.Thread.Sleep(15000)
    Kevo.AppendLog.commit<string> |> ignore
    System.Threading.Thread.Sleep(1500)
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<string>
    printfn "%A" Kevo.Core.getDictionary<string>.Count



  
let testWrapper<'t> query =
 //   printfn "testReadSumAllNones %A " (duration (fun () -> testReadSumAllNones<'t> 100000))
 //   printfn "%A " (duration (fun () -> testStraightWildcardSearch<'t> query))
//    printfn "%A " (duration (fun () -> testDeserializeFromProtoBuf<'t>))
 //     printfn "%A %A" (string typeof<Dictionary<int, 't>>) typeof<Dictionary<int, 't>>.GUID
 //     let a = Kevo.Core.getDictionary<'t>
 //     printfn "%A" a
   //   printfn "%A %A" (string typeof<Dictionary<int, string>>) typeof<Dictionary<int, string>>.GUID
   // printfn "%A" (duration "appenSync" (fun () -> Kevo.AppendLog.appendSync<int> 1 1))
   // printfn "%A" (duration "commit" (fun () -> Kevo.AppendLog.commit<int>)) 
   //   testAppendLoop 100000 10000
       testAppendLoop 10000 1000


   
