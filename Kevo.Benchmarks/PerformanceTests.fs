module Kevo.PerformanceTests

open System.Diagnostics
open Kevo.Store
open Kevo.JsonNet
open System.Collections.Generic


let duration f = 
    let timer = new System.Diagnostics.Stopwatch()
    timer.Start()
    let returnValue = f()
    printfn "%A Ellapsed Time: %f ms" (f.GetType().ToString()) timer.Elapsed.TotalMilliseconds
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

let inline testAppend<'t>=
   // let c = Kevo.Store.getDictionary<'t>    
    [0..10]|> List.map (fun x -> Kevo.Store.append x) |> ignore
    System.Threading.Thread.Sleep(15000)

let testWrapper<'t> query =
 //   printfn "testReadSumAllNones %A " (duration (fun () -> testReadSumAllNones<'t> 100000))
 //   printfn "%A " (duration (fun () -> testStraightWildcardSearch<'t> query))
//    printfn "%A " (duration (fun () -> testDeserializeFromProtoBuf<'t>))
    printfn "%A" (duration (fun () -> testAppend<'t>))
    
