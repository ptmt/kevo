module Kevo.ProtoBuf

open ProtoBuf
open System.Diagnostics

let serialize<'t> what = 
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    use file = System.IO.File.Create(filename)
    Serializer.Serialize<'t>(file, what); 
    stopWatch.Stop()
    printfn "serialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
 

let deserialize<'t> =  
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    use file = System.IO.File.OpenRead(filename)
    let c = Serializer.Deserialize<'t>(file);   
    stopWatch.Stop()   
    printfn "deserialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
    c
