using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace backend.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly DistributedCacheEntryOptions _defaultOptions;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(
        IDistributedCache cache,
        ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _defaultOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                _logger.LogInformation("Getting cache for key: {Key}", key);
                var data = await _cache.GetStringAsync(key);
                
                if (string.IsNullOrEmpty(data))
                {
                    _logger.LogInformation("Cache miss for key: {Key}", key);
                    return default;
                }

                _logger.LogInformation("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(data, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key {Key}", key);
                await RemoveAsync(key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            if (value == null) return;

            try
            {
                _logger.LogInformation("Setting cache for key: {Key}", key);
                var options = expirationTime.HasValue
                    ? new DistributedCacheEntryOptions 
                    { 
                        AbsoluteExpirationRelativeToNow = expirationTime,
                        SlidingExpiration = TimeSpan.FromMinutes(2)
                    }
                    : _defaultOptions;

                var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
                await _cache.SetStringAsync(key, serializedData, options);
                _logger.LogInformation("Cache set successfully for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key {Key}", key);
                await RemoveAsync(key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key {Key}", key);
            }
        }
    }
}