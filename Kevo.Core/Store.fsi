module Kevo.Store

val findById : int-> 't option

val findByQuery : ('t -> bool) -> 't list
