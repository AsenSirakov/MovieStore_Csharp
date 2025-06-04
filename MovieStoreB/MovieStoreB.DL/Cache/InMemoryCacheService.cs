using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using MovieStoreB.Models.DTO;

namespace MovieStoreB.DL.Cache
{
    public interface IInMemoryCacheService<TData, TKey> where TData : CacheItem<TKey>
    {
        Task AddOrUpdate(TData item);
        Task AddOrUpdateBatch(IEnumerable<TData> items);
        Task<TData?> Get(TKey key);
        Task<IEnumerable<TData>> GetAll();
        Task Remove(TKey key);
        Task Clear();
        int Count { get; }
    }

    public class InMemoryCacheService<TData, TKey> : IInMemoryCacheService<TData, TKey>
        where TData : CacheItem<TKey>
        where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, TData> _cache = new();
        private readonly ILogger<InMemoryCacheService<TData, TKey>> _logger;

        public InMemoryCacheService(ILogger<InMemoryCacheService<TData, TKey>> logger)
        {
            _logger = logger;
        }

        public int Count => _cache.Count;

        public Task AddOrUpdate(TData item)
        {
            if (item == null) return Task.CompletedTask;

            _cache.AddOrUpdate(item.GetKey(), item, (key, existingItem) => item);
            _logger.LogDebug("Added/Updated {DataType} with key {Key} in cache", typeof(TData).Name, item.GetKey());

            return Task.CompletedTask;
        }

        public async Task AddOrUpdateBatch(IEnumerable<TData> items)
        {
            if (items == null) return;

            var validItems = items.Where(x => x != null).ToList();

            foreach (var item in validItems)
            {
                await AddOrUpdate(item);
            }

            _logger.LogInformation("Added/Updated {Count} {DataType} items in cache", validItems.Count, typeof(TData).Name);
        }

        public Task<TData?> Get(TKey key)
        {
            _cache.TryGetValue(key, out var item);
            return Task.FromResult(item);
        }

        public Task<IEnumerable<TData>> GetAll()
        {
            return Task.FromResult(_cache.Values.AsEnumerable());
        }

        public Task Remove(TKey key)
        {
            _cache.TryRemove(key, out _);
            _logger.LogDebug("Removed {DataType} with key {Key} from cache", typeof(TData).Name, key);
            return Task.CompletedTask;
        }

        public Task Clear()
        {
            _cache.Clear();
            _logger.LogInformation("Cleared all {DataType} items from cache", typeof(TData).Name);
            return Task.CompletedTask;
        }
    }
}