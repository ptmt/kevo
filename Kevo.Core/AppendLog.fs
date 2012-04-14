﻿module Kevo.AppendLog

open System.Threading
open System.Runtime.Serialization
open System
open Newtonsoft.Json
open System.IO
open ProtoBuf

let dataPath =
    "C:\\data\\append\\"

let getpath typename second = 
    dataPath + typename + (string second) + ".bin"

// datetime, string of type, guid, obj

[<ProtoContract>]
type AppendLog<'t> = 
    System.DateTime * string * string * 't


// json    
let appendJ<'t> o =       
    let append:AppendLog<'t> = DateTime.Now, typeof<'t>.ToString(), "insert", o
    let serializer = new JsonSerializer();        
    use sw = new StreamWriter(getpath (string typeof<'t>) DateTime.Now.Second)
    use writer = new JsonTextWriter(sw)
    serializer.Serialize(writer, append);   

// protobuf 
let appendP<'t> o = 
    use file = new FileStream(getpath (string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog<'t> = DateTime.Now, typeof<'t>.ToString(), "insert", o    
    
    lock file (fun () -> Serializer.SerializeWithLengthPrefix<AppendLog<'t>>(file, append, PrefixStyle.Base128)) 
   

// quick write to append log
let AppendSyncPart<'t> o = 
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
    use file = new FileStream(getpath (string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog<'t> = DateTime.Now, typeof<'t>.ToString(), "insert", o      
    lock file (fun () -> formatter.Serialize(file, append))
    


// slow write to memory and serialize
let AppendAsyncPart<'t> o = 
    async {
        // check memory cache and file                
        let dict = Kevo.Core.getDictionary<'t>
        // add to memory
        dict.Add(dict.Count, o)
        Kevo.Core.saveDictionary dict |> ignore
        // serialize into file
        //Kevo.Core.saveDictionary dict
      //  flushChanges |> ignore        
       
        } |> Async.Start

let getFiles<'t> = 
    System.IO.Directory.GetFiles(dataPath) |> Array.filter (fun x -> x.Contains(string typeof<'t>))

let inline getinfo (_, _, a, b) = a, b

let processItem x = 
    printfn "%A" x

let processFile<'t> (filename:string) = 
    let newfilename = filename.Replace(".bin", ".temp");
    System.IO.File.Move(filename, newfilename)
    use file = System.IO.File.Create(newfilename)
    let items = Serializer.DeserializeItems<AppendLog<'t>>(file, PrefixStyle.Base128, -1) |> List.ofSeq
    file.Close()
    System.IO.File.Delete(newfilename)
    items |> List.map processItem

let commit<'t> =     
    let files = getFiles<'t>  
    if files.Length = 0 || (files |> Array.filter (fun x -> x.Contains(".temp"))).Length > 0 then
        false
    else
        files |> Array.map (fun x-> processFile<'t> x) |> ignore
        true