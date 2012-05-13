﻿module Kevo.MemoryCache

open System.Runtime.Caching

let addToCache itemKey item =
    let cache = MemoryCache.Default;
    lock cache (fun () ->         
        cache.Set(itemKey, item, System.Runtime.Caching.MemoryCache.InfiniteAbsoluteExpiration))

let getFromCache itemKey=
    let cache = MemoryCache.Default;
    cache.Get(itemKey)

let clearCache itemKey = 
    let cache = MemoryCache.Default
    lock cache (fun () -> cache.Remove(itemKey)) |> ignore 
