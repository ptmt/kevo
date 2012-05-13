module Kevo.ProtoBuf

open ProtoBuf
open System.Diagnostics
open System.Reflection

let path filename =
    "C:\\data\\" + filename

let serialize<'t> what where =     
    let stopWatch = Stopwatch.StartNew()
    use file = System.IO.File.Create(path where)
    lock file (fun () -> Serializer.Serialize<'t>(file, what); 
                         file.SetLength(file.Position); // fix truncated 
                         file.Close())
    stopWatch.Stop()
    //printfn "Protobuf serialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
 

let deserialize<'t> from =    
    let stopWatch = Stopwatch.StartNew()
    if (System.IO.File.Exists(path from)) then 
        use file = System.IO.File.OpenRead(path from)
        let c = Serializer.Deserialize<'t>(file);   
        file.Close()
        stopWatch.Stop()   
        //printfn "Protobuf deserialization complete in %f ms" stopWatch.Elapsed.TotalMilliseconds   
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
