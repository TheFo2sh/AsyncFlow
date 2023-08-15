namespace AsyncFlow.Core.Cache;

public interface IAsyncFlowCache
{
    void Set<T>(string key, T value);
    T? Get<T>(string key);
    void Delete(string key);
}