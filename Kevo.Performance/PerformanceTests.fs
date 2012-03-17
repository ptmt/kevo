module Kevo.PerformanceTests

open System.Diagnostics
open Kevo.Store



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


let testWrapper<'t> query =
    printfn "testReadSumAllNones %A " (duration (fun () -> testReadSumAllNones<'t> 100000))
    printfn "%A " (duration (fun () -> testStraightWildcardSearch<'t> query))
    
