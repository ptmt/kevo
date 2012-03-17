#KeVa

[PreviewVersion]

Damn Easy Key-Value Store written in F# 3.0, serializing in Protocol Buffers and using System.Runtinme.MemoryCache.

		let stringEmpty = ""

		[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
		type Address () = class  
			member val Street : string = stringEmpty with get, set
			member val City : string = stringEmpty with get, set  
			member val Country : string = stringEmpty with get, set 		
			member val Syn : int array = [||] with get, set 				
		end  

`Kevo.Store` can find items by Id:

    Kevo.Store.findById<WordItem> id