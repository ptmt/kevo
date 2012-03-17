module Kevo.MemoryCache

open System.Runtime.Caching

let addToCache itemKey item =
    let cache = MemoryCache.Default;
    cache.Add(itemKey, item, System.Runtime.Caching.MemoryCache.InfiniteAbsoluteExpiration)

let getFromCache itemKey=
    let cache = MemoryCache.Default;
    cache.Get(itemKey)

