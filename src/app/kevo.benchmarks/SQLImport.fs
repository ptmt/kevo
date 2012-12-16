module SQLImport

//
//[<ProtoContract>]
//type dict = Dictionary<int, WordItem>

//let connectionString = "Server=MADEINCHINA\SQLEXPRESS;Database=touty;Trusted_Connection=True;"

//let GetSyn (id:int) = 
//      use conn = new DynamicSqlConnection(connectionString)
//      use cmd = conn?GetSYN
//      cmd?index <- id   
//      conn.Open()
//      use reader = cmd.ExecuteReader()
//      [| 
//        while reader.Read() do
//        yield reader?syn |]
//
//  
//let GetWord (id:int) =
//  use conn = new DynamicSqlConnection(connectionString)
//  use cmd = conn?GetWord
//  cmd?index <- id   
//  conn.Open()
//  use reader = cmd.ExecuteReader()
//  reader.Read() |> ignore
//  let gettype =
//      match reader?part with
//        | 1 -> LexicalClass.Noun
//        | 2 -> LexicalClass.Adjective
//        | 3 -> LexicalClass.Adverb
//        | 5 -> LexicalClass.Prepositions
//        | _ -> LexicalClass.Others
//        // (word = reader?word; wordst = reader?wordst;  suff = reader?suff;part = gettype;syn = List.Empty;prefix = reader?prefix} 
//  let a = (GetSyn reader?wid)
//  (reader?wid, (new WordItem (reader?word,reader?wordst,reader?suff,gettype, a,reader?prefix)))
//
//
//
//let serialize =  
//    let c = new dict()       
//    
//    for a in 1 .. 170000 do
//        try
//            let b = GetWord a
//            c.Add b
//        with 
//            | :? System.InvalidOperationException -> printfn "."                
//    Kevo.ProtoBuf.serialize<dict> c  
    
    

//let deserialize =           
//    let c = Kevo.ProtoBuf.deserialize<dict>
//    printfn "%A" (c.Item (2)).Word   
//
//
////serialize
//deserialize
// 
