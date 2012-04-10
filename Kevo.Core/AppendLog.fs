module Kevo.AppendLog

open System.Threading
open System.Runtime.Serialization
open System
open Newtonsoft.Json
open System.IO
open ProtoBuf

let path second =
    "C:\\data\\append" + string second + ".bin"

let pathTemp = 
    "C:\\data\\append.temp"

// datetime, string of type, guid, obj
[<Serializable>]
[<ProtoContract>]
type AppendLog = 
    System.DateTime * string * string * obj

let flushChanges =
    match System.IO.File.Exists(path DateTime.Now.Second), System.IO.File.Exists(pathTemp) with
       | true, false -> System.IO.File.Move(path DateTime.Now.Second, pathTemp)
                        Thread.Sleep(10000)  
                        System.IO.File.Delete(pathTemp)
                        printfn "awake"   
                        true
       | _ -> printfn "lock or nothing"
              false
       
   
let appendJ<'t> o =   
    //let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
    //use file = System.IO.File.OpenWrite(path)
    let append:AppendLog = DateTime.Now, typeof<'t>.ToString(), typeof<'t>.GUID.ToString(), o
    let serializer = new JsonSerializer();        
    use sw = new StreamWriter(path DateTime.Now.Second)
    use writer = new JsonTextWriter(sw)
    serializer.Serialize(writer, append);
    //formatter.Serialize(file, append)    
    //printfn "%A" o
    o

let appendP<'t> o = 
    use file = new FileStream(path DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog = DateTime.Now, typeof<'t>.ToString(), typeof<'t>.GUID.ToString(), o    
    lock file (fun () -> Serializer.Serialize<AppendLog>(file, append)) 
   

// quick write to append log
let AppendSyncPart<'t> o = 
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
    use file = new FileStream(path DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog = DateTime.Now, typeof<'t>.ToString(), typeof<'t>.GUID.ToString(), o    
  
    lock file (fun () -> formatter.Serialize(file, append))
    //printfn "%A" o
    //o


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


    



