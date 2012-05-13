module Kevo.AppendLog

open System.Threading
open System.Runtime.Serialization
open System
open Newtonsoft.Json
open System.IO
open ProtoBuf

let dataPath =
    "C:\\data\\append\\"

let inline getpath typename second = 
    dataPath + typename + (string second) + ".bin"

// datetime, string of type, guid, obj

[<ProtoContract>]
type AppendLog<'t> = 
    int * string * string * 't


// json  
// DEPRECATED  
let inline appendJ<'t> o =    

    let dict = Kevo.Core.getDictionary<'t>      
    let append:AppendLog<'t> = dict.Count, typeof<'t>.ToString(), "insert", o
    let serializer = new JsonSerializer();        
    use sw = new StreamWriter(getpath (string typeof<'t>) DateTime.Now.Second)
    use writer = new JsonTextWriter(sw)
    serializer.Serialize(writer, append);   

// protobuf 
let inline appendCommon<'t> index o operation_type = 
    use file = new FileStream(getpath (string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog<'t> = index, typeof<'t>.ToString(), operation_type, o        
    lock file (fun () -> Serializer.SerializeWithLengthPrefix<AppendLog<'t>>(file, append, PrefixStyle.Base128)
                         file.Close()) 


let appendSync<'t> index o =     
    appendCommon<'t> index o "insert"

let updateSync<'t> index o = 
    appendCommon<'t> index o "update"
    
   

// quick write to append log
// DEPRECATED
let inline AppendSyncBinary<'t> o = 
    let dict = Kevo.Core.getDictionary<'t>     
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
    use file = new FileStream(getpath (string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog<'t> = dict.Count, typeof<'t>.ToString(), "insert", o      
    lock file (fun () -> formatter.Serialize(file, append))
    



let getFiles<'t> = //
    System.IO.Directory.GetFiles(dataPath) |> Array.filter (fun x -> x.Contains(string typeof<'t>) && x.Contains(string DateTime.Now.Second) = false)

let inline getinfo (i, _, a, b) = i, a, b

let inline saveToDict<'t> index item op = 
    let dict = Kevo.Core.getDictionary<'t>
    match op with 
        | "insert" ->
                 if dict.ContainsKey(index) = false then
                    dict.Add(index, item)
        | "update" -> 
                 if dict.ContainsKey(index) = false then
                    dict.Add(index, item)
                 else
                    dict.[index] <- item
        | _ -> failwith "unsupported operation"  
    Kevo.Core.saveDictionary dict |> ignore

let inline processItem<'t> x = 
    let index, op, b = getinfo x    
    saveToDict<'t> index b op
    

let inline renameFile (filename:string) = 
    let newfilename = filename.Replace(".bin", ".temp");
    if System.IO.File.Exists(newfilename) = false then
         System.IO.File.Move(filename, newfilename)
    newfilename

let inline processFile<'t> (filename:string) = 
    //printfn "ad"
    if System.IO.File.Exists(filename) then
        use file = System.IO.File.OpenRead(filename)
        // lock file (fun () -> 
        //                      )
        let items = Serializer.DeserializeItems<AppendLog<'t>>(file, PrefixStyle.Base128, 0) |> List.ofSeq        
        file.Close()
        System.IO.File.Delete(filename)
        items |> List.map (fun x -> processItem<'t> x) |> ignore
        let dict = Kevo.Core.getDictionary<'t>        
        Kevo.ProtoBuf.serialize<System.Collections.Generic.Dictionary<int, 't>> dict (string typeof<'t>.GUID)
        true
    else
        false

let commit<'t> =     
    let files = getFiles<'t> //|> List.ofArray
    if files.Length = 0 || (files |> Array.filter (fun x -> x.Contains(".temp"))).Length > 0 then
        false
    else
        try 
            files 
            |> Array.map renameFile 
            |> Array.map processFile<'t> 
            |> Array.filter (fun x-> x = false) 
            |> Array.length = 0 
        with
            | :? System.IO.IOException ->
                 printfn "io exception"
                 false
        


// slow write to memory and serialize
let appendAsync<'t> index o = 
    async {        
        let dict = Kevo.Core.getDictionary<'t>     
        
        lock dict (fun () -> 
                    if dict.ContainsKey(index) = false then
                        dict.Add(index, o)
                        Kevo.Core.saveDictionary dict |> ignore
                        commit<'t> |> ignore
                    else
                        commit<'t> |> ignore
                    
                    )
        
    } |> Async.Start 

let updateAsync<'t> index o =
    async {        
        let dict = Kevo.Core.getDictionary<'t>     
        
        lock dict (fun () -> if dict.ContainsKey(index) = false then
                                dict.Add(index, o)
                                Kevo.Core.saveDictionary dict |> ignore
                             else
                                dict.[index] <- o
                                Kevo.Core.saveDictionary dict |> ignore
                             commit<'t> |> ignore )
               
    } |> Async.Start 