#KeVo

Damn Easy Key-Value Store written in F# 3.0. 

- serializing all Object in Protocol Buffers or Json.NET
- using System.Runtinme.MemoryCache 

##Getting Started

`Kevo.Store` contains two function for retrieve items:

	val findById : int-> 't option

	val findByQuery : ('t -> bool) -> 't list

Let's say you have a class for storing your data:

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

##Benchmarks

Few tests from: `Kevo.Benchmark`

For Intel Core i5 2500K / 16gb RAM:


	Protobuf deserialization complete in 1431.623400 ms
	Protobuf serialization complete in 311.327300 ms
	Json.Net serialization complete in 1770.890400 ms
	Json.Net deserialization complete in 3060.234400 ms
	"Kevo.PerformanceTests+testWrapper@24[Kevo.Performance+WordItem]" Ellapsed Time: 2291.827700 ms
	testReadSumAllNones 564674759
	"Kevo.PerformanceTests+testWrapper@25-3[Kevo.Performance+WordItem]" Ellapsed Time: 66.317000 ms	