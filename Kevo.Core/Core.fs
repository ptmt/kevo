module Kevo.Core

open Kevo.MemoryCache
open Kevo.ProtoBuf
open System.Collections.Generic
open ProtoBuf

//[<ProtoContract>]
//type dict = Dictionary<int, 't>

let inline cacheIndex<'t> = typeof<'t>.GUID.ToString();

let getDictionary<'t> =
    let a = getFromCache cacheIndex<'t> 
    match a with         
        | null -> let b = deserialize<Dictionary<int, 't>>
                  addToCache cacheIndex<'t> b |> printfn "added to cache %A"
                  b
        | _ -> a :?> Dictionary<int, 't>

let saveDictionary<'t> dict =
     serialize<Dictionary<int, 't>> dict