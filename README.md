#KeVa[PreviewVersion]



Damn Easy Key-Value Store written in F# 3.0. KeVo's features:

- serializing all Object in Protocol Buffers
- using System.Runtinme.MemoryCache 

##Getting Started

`Kevo.Store` contains two function for retrieve items:

	val findById : int-> 't option

	val findByQuery : ('t -> bool) -> 't list

Let's say You have a class for stored your data:

	let stringEmpty = ""

	[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
	type Address () = class  
		member val Street : string = stringEmpty with get, set
		member val City : string = stringEmpty with get, set  
		member val Country : string = stringEmpty with get, set 		
		member val Syn : int array = [||] with get, set 				
	end  

To find any item by id, use findById:
    
	let myAddress = Kevo.Store.findById<Address> id

Use predicate function for search by any templates:

	let query (x:WordItem) =
		x.Wordst.Contains("слово")
	Kevo.Store.findByQuery<Address> query |> printfn "%A"

##Roadmap

- Indexeses
- Write-log
- Tests

##Performance

Few tests here: `Kevo.Performance`

For Intel Core i5 2500K / 16gb RAM:


	deserialization complete in 1403.930400 ms
	added to cache true
	"Kevo.PerformanceTests+testWrapper@24[Kevo.Performance+WordItem]" Ellapsed Time: 2291.827700 ms
	testReadSumAllNones 564674759
	"Kevo.PerformanceTests+testWrapper@25-3[Kevo.Performance+WordItem]" Ellapsed Time: 66.317000 ms	