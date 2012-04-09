module Kevo.Store

open System.Collections.Generic

val findById<'t> : int -> 't option

val findByQuery<'t> : ('t -> bool) -> 't list

val append<'t> : 't -> unit

val delete<'t> : int -> unit

