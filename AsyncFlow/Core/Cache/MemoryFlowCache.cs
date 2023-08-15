using Microsoft.Extensions.Caching.Memory;

namespace AsyncFlow.Core.Cache;

public class MemoryFlowCache : IAsyncFlowCache
{
    private readonly IMemoryCache _memoryCache;

    public MemoryFlowCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Set<T>(string key, T value)
    {
        _memoryCache.Set(key, value);
    }

    public T? Get<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out T value) ? value : default;
    }

    public void Delete(string key)
    {
        _memoryCache.Remove(key);
    }
}