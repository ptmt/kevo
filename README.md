#KeVo

KeVo is a Damn Easy Key-Value Store written in F# 3.0. It is in-memory store using System.Runtime.MemoryCache with full synchronized append log.

It serialize all objects in Protocol Buffers (Json.NET or binary serialization optional)


##Getting Started

`Kevo.Store` contains two function for retrieve items:

	val findById : int-> 't option

	val findByQuery : ('t -> bool) -> 't list

insert items:
	
	val append<'t> :  int * 't * (unit -> unit) option -> unit

update

	val update<'t> :  int * 't  -> unit

##Create/insert

By default if storage is not exist `append` function create new file. For example, to insert any string in Dictionary<int, string> just use `append` function:

	let add_string index =
		Kevo.Store.append<string>(index, (index x), None)

##Read

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
		x.Wordst.Contains("Local Street")
	Kevo.Store.findByQuery<Address> query |> printfn "%A"


##Benchmarks

Few tests from `Kevo.Benchmark`.

Test machine configuration is Intel Core i5 2500K / 16gb RAM:


	Protobuf deserialization complete in 1431.623400 ms
	Protobuf serialization complete in 311.327300 ms
	Json.Net serialization complete in 1770.890400 ms
	Json.Net deserialization complete in 3060.234400 ms	
	testReadSumAllNones 564674759
	

### Append/Update


![benchmark screenshot](https://github.com/unknownexception/kevo/raw/master/Kevo.Benchmarks/screenshot1.png)