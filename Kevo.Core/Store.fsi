module Kevo.Store

open System.Collections.Generic

val findById<'t> : int -> 't option

val findByQuery<'t> : ('t -> bool) -> 't list

val append<'t> :  int * 't * (unit -> unit) option -> unit

val update<'t> :  int * 't  -> unit

val size<'t> : int

val lastid<'t> : int

val delete<'t> :  int -> unit

val memo : (unit -> 't) -> 't
