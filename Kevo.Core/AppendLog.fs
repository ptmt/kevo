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
type AppendLog<'t> = 
    int * string * string * 't


// json    
let appendJ<'t> o =    

    let dict = Kevo.Core.getDictionary<'t>      
    let append:AppendLog<'t> = dict.Count, typeof<'t>.ToString(), "insert", o
    let serializer = new JsonSerializer();        
    use sw = new StreamWriter(getpath (string typeof<'t>) DateTime.Now.Second)
    use writer = new JsonTextWriter(sw)
    serializer.Serialize(writer, append);   

// protobuf 
let appendSync<'t> index o =     
    use file = new FileStream(getpath (string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog<'t> = index, typeof<'t>.ToString(), "insert", o        
    lock file (fun () -> Serializer.SerializeWithLengthPrefix<AppendLog<'t>>(file, append, PrefixStyle.Base128)) 
    
   

// quick write to append log
let AppendSyncBinary<'t> o = 
    let dict = Kevo.Core.getDictionary<'t>     
    let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
    use file = new FileStream(getpath (string typeof<'t>) DateTime.Now.Second,  FileMode.Append, FileAccess.Write, FileShare.None)
    let append:AppendLog<'t> = dict.Count, typeof<'t>.ToString(), "insert", o      
    lock file (fun () -> formatter.Serialize(file, append))
    



let getFiles<'t> = 
    System.IO.Directory.GetFiles(dataPath) |> Array.filter (fun x -> x.Contains(string typeof<'t>))

let inline getinfo (i, _, a, b) = i, a, b

let inline processItem<'t> x = 
    let index, a, b = getinfo x
    match a with 
        | "insert" -> let dict = Kevo.Core.getDictionary<'t>
                      if dict.ContainsKey(index) = false then                        
                        dict.Add(index, b)
                        Kevo.Core.saveDictionary dict |> ignore
                      Kevo.ProtoBuf.serialize<System.Collections.Generic.Dictionary<int, 't>> dict
        | _ -> printfn "unsupported operation"

let inline processFile<'t> (filename:string) = 
    let newfilename = filename.Replace(".bin", ".temp");
    System.IO.File.Move(filename, newfilename)
    use file = System.IO.File.OpenRead(newfilename)
    lock file (fun () -> let items = Serializer.DeserializeItems<AppendLog<'t>>(file, PrefixStyle.Base128, 0) |> List.ofSeq
                         file.Close()
                         System.IO.File.Delete(newfilename)
                         items |> List.map (fun x -> processItem<'t> x))
   

let commit<'t> =     
    let files = getFiles<'t>  
    if files.Length = 0 || (files |> Array.filter (fun x -> x.Contains(".temp"))).Length > 0 then
        false
    else
        files |> Array.map (fun x-> processFile<'t> x) |> ignore
        true


// slow write to memory and serialize
let appendAsync<'t> index o = 
    async {        
        let dict = Kevo.Core.getDictionary<'t>      
        if dict.ContainsKey(index) = false then
            dict.Add(index, o)
            Kevo.Core.saveDictionary dict |> ignore
            commit<'t> |> ignore          
    } |> Async.Start