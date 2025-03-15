using Microsoft.Extensions.Caching.Memory;

public class CustomMemoryCaches
{
    private readonly IMemoryCache _cache;

    public CustomMemoryCaches(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Set(string key, object value, TimeSpan expiration)
    {
        _cache.Set(key, value, expiration);
    }

    public bool TryGetValue(string key, out object value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}