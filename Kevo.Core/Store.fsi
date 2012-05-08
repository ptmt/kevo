module Kevo.Store

open System.Collections.Generic

val findById<'t> : int -> 't option

val findByQuery<'t> : ('t -> bool) -> 't list

val append<'t> :  int * 't * (unit -> unit) option -> unit

val delete<'t> :  int -> unit

val memo : (unit -> obj) -> obj
