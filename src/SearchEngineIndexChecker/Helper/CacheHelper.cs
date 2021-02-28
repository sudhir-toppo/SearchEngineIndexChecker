
using System;
using System.Runtime.Caching;


namespace SearchEngineIndexChecker.Helper
{
    public interface ICacheHelper
    {
        public void Add(string key, object obj);
        public object Get(string name);
        public T Get<T>(string name);
    }
    public class CacheHelper : ICacheHelper
    {
        static MemoryCache cache = new MemoryCache("CachingProvider");
        public void Add(string key, object obj)
        {
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now.AddHours(1)
            };
            cache.Add(key, obj, cacheItemPolicy);
        }

        public object Get(string name)
        {
            return cache.Get(name);
        }

        public T Get<T>(string name)
        {
            return (T)cache.Get(name);
        }
    }
}
