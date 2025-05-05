using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using Core.Application;
using Core.CrossCuttingConcerns.Logger;
using Serilog;

namespace ETicaret.Application.Services.RedisServices;

public class RedisCasheService : IRedisService
{
    private readonly IDistributedCache _distributedCache;
    private readonly CachingConfiguration _cachingConfiguration;
    private readonly ILoggerService _loggerService;
    private readonly ILogger _logger = Log.Logger;

    public RedisCasheService(IDistributedCache distributedCache, CachingConfiguration cachingConfiguration, ILoggerService loggerService)
    {
        _distributedCache = distributedCache;
        _cachingConfiguration = cachingConfiguration;
        _loggerService = loggerService;
    }

    public async Task AddDataAsync<T>(string key, T value)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(value);

            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(_cachingConfiguration.SlidingExpiration),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cachingConfiguration.AbsoluteExpiration)
            };

            await _distributedCache.SetAsync(key, dataBytes, options);
        }
        catch (Exception ex)
        {
            _loggerService.Error("RedisCacheService AddDataAsync error: " + ex.Message, ex);
            //_logger.Error("RedisCacheService AddDataAsync error: " + ex.Message);

            throw new Exception("RedisCacheService AddDataAsync error: " + ex.Message);
        }
    }

    public async Task<T> GetDataAsync<T>(string key)
    {
        try
        {
            byte[] datas = await _distributedCache.GetAsync(key);

            if (datas == null)
                return default;

            var jsonData = Encoding.UTF8.GetString(datas);

            T response = JsonSerializer.Deserialize<T>(jsonData);

            return response;
        }
        catch (Exception ex)
        {
            _loggerService.Error("RedisCacheService GetDataAsync error: " + ex.Message, ex);
            //_logger.Error("RedisCacheService GetDataAsync error: " + ex.Message);

            throw new Exception("RedisCacheService GetDataAsync error: " + ex.Message);
        }
    }

    public async Task RemoveDataAsync(string key)
    {
        try
        {
            await _distributedCache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _loggerService.Error("RedisCacheService RemoveDataAsync error: " + ex.Message, ex);
            //_logger.Error("RedisCacheService RemoveDataAsync error: " + ex.Message);

            throw new Exception("RedisCacheService RemoveDataAsync error: " + ex.Message);
        }
    }
}
