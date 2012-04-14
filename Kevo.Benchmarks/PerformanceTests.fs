module Kevo.PerformanceTests

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
    Kevo.ProtoBuf.serialize<Dictionary<int, 't>> c
    Kevo.JsonNet.serialize<Dictionary<int, 't>> c
    let b = Kevo.JsonNet.deserialize<Dictionary<int, 't>>    
    b


let inline testAppend c =   
   let timer = new System.Diagnostics.Stopwatch()
   timer.Start()
   duration (fun () -> [0..c]|> List.map (fun x -> Kevo.Store.append<string> (string x)) |> ignore)   
   let perf = float c / timer.Elapsed.TotalMilliseconds
   printfn "%i; %f" c perf

   
let testAppendLoop lmax step= 
    [0..step..lmax] |> List.map (fun x -> testAppend x) |> ignore
    //System.Threading.Thread.Sleep(15000)



  
let testWrapper<'t> query =
 //   printfn "testReadSumAllNones %A " (duration (fun () -> testReadSumAllNones<'t> 100000))
 //   printfn "%A " (duration (fun () -> testStraightWildcardSearch<'t> query))
//    printfn "%A " (duration (fun () -> testDeserializeFromProtoBuf<'t>))
    printfn "%A" (duration "appenSync" (fun () -> Kevo.AppendLog.appendSync<int> 1))
    printfn "%A" (duration "commit" (fun () -> Kevo.AppendLog.commit<int>)) 
 //   testAppendLoop 100000 10000
   
