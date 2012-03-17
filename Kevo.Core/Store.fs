module Kevo.Store

open Kevo.MemoryCache
open Kevo.ProtoBuf
open System.Collections.Generic
open System.Linq

let inline cacheIndex<'t> = typeof<'t>.GUID.ToString();

let inline getDictionary<'t> =
    let a = getFromCache cacheIndex<'t> 
    match a with         
        | null -> let b = deserialize<Dictionary<int, 't>>
                  addToCache cacheIndex<'t> b |> printfn "added to cache %A"
                  b
        | _ -> a :?> Dictionary<int, 't>

let findById<'t> id =
    let dict = getDictionary<'t>
    match dict.ContainsKey id with
        | true -> Some(dict.Item id)
        | false -> None

let alllist<'t> = 
    getDictionary<'t>.Values.ToArray()    

let findByQuery query =
    alllist<'t> |> List.ofArray |> List.filter query
    
let findWhere (query:System.Func<KeyValuePair<int,'t>, bool>) =
    getDictionary.Where(query)


