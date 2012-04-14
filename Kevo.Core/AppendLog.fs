module Kevo.AppendLog

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
type AppendLog = 
    System.DateTime * string * string * obj


// json    
let appendJ<'t> o =       
    let append:AppendLog = DateTime.Now, typeof<'t>.ToString(), "insert", o
    let serializer = new JsonSerializer();        
    use sw = new StreamWriter(getpath (string typeof<'t>) DateTime.Now.Second)
    use writer = new JsonTextWriter(sw)
    serializer.Serialize(writer, append);   

// protobuf 
let appendP<'t> o = 
    use file = new FileStream(getpath (string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog = DateTime.Now, typeof<'t>.ToString(), "insert", o    
    
    lock file (fun () -> Serializer.SerializeWithLengthPrefix<AppendLog>(file, append, PrefixStyle.Base128)) 
   

// quick write to append log
let AppendSyncPart<'t> o = 
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
    use file = new FileStream(getpath ((string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog = DateTime.Now, typeof<'t>.ToString(), "insert", o      
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


    