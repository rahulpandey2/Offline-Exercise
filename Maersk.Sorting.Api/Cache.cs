using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public static class Caching
    {
        static readonly object cacheLock = new object();

       
        //public static T GetOrCreate<T>(string key, DateTimeOffset dateTimeOffset, Func<T> valueFactory)
        //{
        //    ObjectCache cache = MemoryCache.Default;

        //    object cachedObject = cache[key];

        //    if (cachedObject == null)
        //    {
        //        lock (cacheLock)
        //        {
        //            if (cachedObject == null)
        //            {
        //                cachedObject = valueFactory();
        //                cache.Set(key, cachedObject, dateTimeOffset);
        //            }
        //        }
        //    }

        //    return (T)cachedObject;
        //}
    }
}
