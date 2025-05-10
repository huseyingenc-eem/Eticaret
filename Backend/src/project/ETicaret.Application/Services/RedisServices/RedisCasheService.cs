using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using Core.Application;
using Core.CrossCuttingConcerns.Logger;
using Serilog;
using Microsoft.Extensions.Options;
using System.Threading;

namespace ETicaret.Application.Services.RedisServices;

public class RedisCasheService : IRedisService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILoggerService _loggerService;

    public RedisCasheService(IDistributedCache distributedCache, ILoggerService loggerService)
    {
        _distributedCache = distributedCache;
        
        _loggerService = loggerService;
    }

    public async Task AddDataAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(value);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);


            await _distributedCache.SetAsync(key, dataBytes, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _loggerService.Error($"RedisCacheService AddDataAsync error for key {key}: {ex.Message}", ex);
            throw new Exception($"RedisCacheService AddDataAsync error. See inner exception for details. Key: {key}", ex);
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
        catch (JsonException jsonEx)
        {
            _loggerService.Error($"RedisCacheService GetDataAsync JSON deserialization error for key {key}: {jsonEx.Message}", jsonEx);
            // Cache'deki veri bozuk olabilir, bu durumda silmek bir seçenek olabilir.
            await RemoveDataAsync(key); // Opsiyonel: Bozuk veriyi temizle
            throw new Exception($"RedisCacheService GetDataAsync error: Could not deserialize data for key {key}. See inner exception.", jsonEx);
        }
        catch (Exception ex)
        {
            _loggerService.Error($"RedisCacheService GetDataAsync error for key {key}: {ex.Message}", ex);
            throw new Exception($"RedisCacheService GetDataAsync error. See inner exception for details. Key: {key}", ex);
        }

    }

    public async Task RemoveDataAsync(string key)
    {
        try
        {
            await _distributedCache.RemoveAsync(key);
            _loggerService.Info($"Data removed from cache with key: {key}");
        }
        catch (Exception ex)
        {
            _loggerService.Error($"RedisCacheService RemoveDataAsync error for key {key}: {ex.Message}", ex);
            throw new Exception($"RedisCacheService RemoveDataAsync error. See inner exception for details. Key: {key}", ex);
        }
    }
}