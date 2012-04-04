module Kevo.ProtoBuf

open ProtoBuf
open System.Diagnostics

let path filename =
    "C:\\data\\" + filename

let serialize<'t> what = 
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    use file = System.IO.File.Create(path filename)
    Serializer.Serialize<'t>(file, what); 
    stopWatch.Stop()
    printfn "serialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
 

let deserialize<'t> =  
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    use file = System.IO.File.OpenRead(path filename)
    let c = Serializer.Deserialize<'t>(file);   
    stopWatch.Stop()   
    printfn "deserialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
    c
