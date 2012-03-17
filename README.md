#KeVa[PreviewVersion]



Damn Easy Key-Value Store written in F# 3.0, serializing in Protocol Buffers and using System.Runtinme.MemoryCache.

##Getting Started

	let stringEmpty = ""

	[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
	type Address () = class  
		member val Street : string = stringEmpty with get, set
		member val City : string = stringEmpty with get, set  
		member val Country : string = stringEmpty with get, set 		
		member val Syn : int array = [||] with get, set 				
	end  


`Kevo.Store` contains two function for retrieve items:

	val FindById : int-> 't option

	val FindByQuery : ('t -> bool) -> 't list


You can find items by Id.:
    
	let myAddress = Kevo.Store.findById<Address> id

It can find items by any predicate:

	let query (x:WordItem) =
		x.Wordst.Contains("слово")
	Kevo.Store.findByQuery<Address> query |> printfn "%A"


##Performance

See: PerformanceTests

For Intel Core i5 2500K / 16gb RAM:


	deserialization complete in 1403.930400 ms
	added to cache true
	"Kevo.PerformanceTests+testWrapper@24[Kevo.Performance+WordItem]" Ellapsed Time: 2291.827700 ms
	testReadSumAllNones 564674759
	"Kevo.PerformanceTests+testWrapper@25-3[Kevo.Performance+WordItem]" Ellapsed Time: 66.317000 ms	