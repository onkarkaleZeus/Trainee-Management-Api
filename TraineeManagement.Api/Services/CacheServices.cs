using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Settings;
using StackExchange.Redis;

namespace TraineeManagement.Api.Services
{
    public class CacheService : ICacheService
    {

        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly RedisSettings _settings;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public CacheService(
            IDistributedCache cache,
            ILogger<CacheService> logger,
            IOptions<RedisSettings> settings
        )
        {
            _cache = cache;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("Key cannot be null or empty", nameof(key));
                }

                var cacheValue = await _cache.GetStringAsync(key);
                if (string.IsNullOrWhiteSpace(cacheValue))
                {
                    _logger.LogWarning("Cache miss for key {key}", key);
                    return default;
                }

                _logger.LogInformation("cache hit for key { key }", key);
                return JsonSerializer.Deserialize<T>(cacheValue, _options);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception encountered while getting from cache {ex}", ex.Message);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T t)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("Key cannot be null or empty", nameof(key));
                }

                var serializedData = JsonSerializer.Serialize(t);
                var options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.TTL),
                    SlidingExpiration = TimeSpan.FromMinutes(_settings.SlidingExpiration)
                };

                await _cache.SetStringAsync(key, serializedData, options);
                _logger.LogInformation("Cache set for key: {key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Exception encountered while setting to cache");
                return;
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("Key cannot be null or empty", nameof(key));
                }

                await _cache.RemoveAsync(key);
                _logger.LogInformation("Cache entry removed for key: {key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Exception encountered while setting to cache");
                return;
            }

        }
    }
}