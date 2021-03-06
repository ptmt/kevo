﻿module Kevo.Store


open System.Collections.Generic
open System.Linq
open Kevo.Core


let findById<'t> id =
    let dict = getDictionary<'t>
    match dict.ContainsKey id with
        | true -> Some(dict.Item id)
        | false -> None

let alllist<'t> = 
    getDictionary<'t>.Values.ToArray()    

let findByQuery<'t> query =
    alllist<'t> |> List.ofArray |> List.filter query
    
let findWhere (query:System.Func<KeyValuePair<int,'t>, bool>) =
    getDictionary.Where(query)

type a = unit->unit

let append<'t> (i:int, o:'t, f:a option) =     
    Kevo.AppendLog.appendSync<'t> i o
    if f.IsSome then f.Value()
    Kevo.AppendLog.appendAsync<'t> i o

let update<'t> (i:int, o:'t) =    
    Kevo.AppendLog.updateSync<'t> i o
    Kevo.AppendLog.updateAsync<'t> i o

let size<'t> = 
    Kevo.Core.getDictionary<'t>.Count    

let lastid<'t> =   
    let dict:Dictionary<int, 't> = Kevo.Core.getDictionary<'t>
    if dict = null || dict.Count = 0 then
        0
    else
        dict.Max(fun (x:KeyValuePair<int, 't>) -> x.Key)
        
let memo<'t> f =     
    let cacheIndex = (f.GetType().GUID.ToString())
    let a = Kevo.MemoryCache.getFromCache cacheIndex
    match a with         
        | null -> let function_result = f()
                  Kevo.MemoryCache.addToCache cacheIndex function_result |> ignore
                  function_result
        | _ -> a :?> 't

let delete<'t> (id:int) = 
    Kevo.AppendLog.deleteSync<'t> id 
    //Kevo.AppendLog.deleteAsync<'t> id

