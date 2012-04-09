module Kevo.JsonNet

open Newtonsoft.Json
open System.Diagnostics
open System.IO

let path filename =
    "C:\\data\\" + filename + ".json"

let serialize<'t> that = 
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    //use file = System.IO.File.Create(path filename)
    let serializer = new JsonSerializer();       
 
    use sw = new StreamWriter(path filename)
    use writer = new JsonTextWriter(sw)
    serializer.Serialize(writer, that);
    
    //Serializer.Serialize<'t>(file, what); 
    stopWatch.Stop()
    printfn "Json.Net serialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
 

let deserialize<'t> =  
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    use file = System.IO.File.OpenRead(path filename)
    let reader = new StreamReader(file)
    let c = JsonConvert.DeserializeObject<'t>(reader.ReadToEnd())    
    stopWatch.Stop()   
    printfn "Json.Net deserialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
    c
