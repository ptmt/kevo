module Kevo.Performance 

open System.Runtime.Caching
open System.Collections.Generic
open System.Collections.Concurrent
open ProtoBuf
open Kevo.ProtoBuf
open System.Diagnostics
open FSharp.Data.DynamicSql
open Kevo.Store
open Kevo.PerformanceTests
open Kevo.ProtoBuf

//select top 1 row_id, fwid, fword, fbsw, fsuff, fpart, fcdpt, fsyn, fcmp, fpref, fnmpart, fokof, fqsyn from sbsw
type LexicalClass =
    | Noun = 1 // существительное
    | Verb = 2 // глагол
    | Adverb = 3 // наречие
    | Adjective = 4 // прилагательное
    | Prepositions = 5 // предлоги
    | Others = 10 

let empty_string = ""

[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
type WordItem (word : string, wordst : string, suff : string, part : LexicalClass, syn : int array, prefix : string) = class  
    member val Word : string = word with get, set
    member val Wordst : string = wordst with get, set  
    member val Suff : string = suff with get, set 
    member val Part : LexicalClass = part with get, set 
    member val Syn : int array = syn with get, set 
    member val Prefix : string = prefix with get, set     
    new() = WordItem(empty_string, empty_string, empty_string, LexicalClass.Others, [||], empty_string)
    override x.ToString() = x.Syn |> List.ofArray |> List.fold (fun x y -> x + " " + y.ToString()) ""
 end  


let query (x:WordItem) =
    x.Wordst.Contains("слово")
//    match x.Wordst with
//    | "слово" -> true
//    | _ -> false

//let query (x:WordItem) =


Kevo.PerformanceTests.testWrapper<WordItem> query
