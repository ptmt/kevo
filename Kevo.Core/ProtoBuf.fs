module Kevo.ProtoBuf

open ProtoBuf
open System.Diagnostics
open System.Reflection

let path filename =
    "C:\\data\\" + filename

let serialize<'t> what = 
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    use file = System.IO.File.Create(path filename)
    Serializer.Serialize<'t>(file, what); 
    stopWatch.Stop()
    printfn "Protobuf serialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
 

let deserialize<'t> =  
    let filename = typeof<'t>.GUID.ToString();
    let stopWatch = Stopwatch.StartNew()
    if (System.IO.File.Exists(path filename)) then 
        use file = System.IO.File.OpenRead(path filename)
        let c = Serializer.Deserialize<'t>(file);   
        stopWatch.Stop()   
        printfn "Protobuf deserialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
        c
    else        
        let t = System.Activator.CreateInstance(typeof<'t>)
        t :?> 't
      
        

//var model = TypeModel.Create();
//
//model.Add(typeof(Task), true);
//
//var compiledModel = model.Compile(path);
//
//compiledModel.Serialize(file, tasks);
