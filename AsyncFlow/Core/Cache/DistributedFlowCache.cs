using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace AsyncFlow.Core.Cache;

public class DistributedFlowCache : IAsyncFlowCache
{
    private readonly IDistributedCache _distributedCache;

    public DistributedFlowCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public void Set<T>(string key, T value)
    {
        var jsonData = JsonSerializer.Serialize(value);
        _distributedCache.SetString(key, jsonData);
    }

    public T? Get<T>(string key)
    {
        var jsonData = _distributedCache.GetString(key);
        return jsonData is null ? default : JsonSerializer.Deserialize<T>(jsonData);
    }

    public void Delete(string key)
    {
        _distributedCache.Remove(key);
    }
}