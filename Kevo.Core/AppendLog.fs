module Kevo.AppendLog

open System.Threading
open System.Runtime.Serialization
open System

let path =
    "C:\\data\\append.bin"

let pathTemp = 
    "C:\\data\\append.temp"

// datetime, string of type, guid, obj
[<Serializable>]
type AppendLog = 
    System.DateTime * string * string * obj

let flushChanges =
    match System.IO.File.Exists(path), System.IO.File.Exists(pathTemp) with
       | true, false -> System.IO.File.Move(path, pathTemp)
                        Thread.Sleep(10000)  
                        System.IO.File.Delete(pathTemp)
                        true
       | _ -> printfn "lock or nothing"
              false
       
      

// quick write to append log
let AppendSyncPart<'t> o = 
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
    use file = System.IO.File.OpenWrite(path)
    let append:AppendLog = DateTime.Now, typeof<'t>.ToString(), typeof<'t>.GUID.ToString(), o
    formatter.Serialize(file, append)    
    printfn "%A" o
    o


// slow write to memory and serialize
let AppendAsyncPart<'t> o = 
    async {
        // check memory cache and file                
        let dict = Kevo.Core.getDictionary<'t>
        // add to memory
        dict.Add(dict.Count, o)
        // serialize into file
        Kevo.Core.saveDictionary dict
        flushChanges |> ignore        
        printfn "awake"   
        } |> Async.Start


    



