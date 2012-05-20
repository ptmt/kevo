module Kevo.MemoryCache

open System.Runtime.Caching

let cache = MemoryCache.Default;

let addToCache itemKey item =   
    lock cache (fun () ->     
        cache.Remove(itemKey) |> ignore    
        cache.Set(itemKey, item, System.Runtime.Caching.MemoryCache.InfiniteAbsoluteExpiration))

let getFromCache itemKey=    
    cache.Get(itemKey)

let clearCache itemKey =     
    lock cache (fun () -> cache.Remove(itemKey)) |> ignore 

let checkCache itemKey = 
    cache.Contains(itemKey)
