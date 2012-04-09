module Kevo.Store


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


let append<'t> (o:'t) =     
        Kevo.AppendLog.AppendSyncPart o |> Kevo.AppendLog.AppendAsyncPart        
        

let delete<'t> (id:int) = 
    printfn "deleting"
