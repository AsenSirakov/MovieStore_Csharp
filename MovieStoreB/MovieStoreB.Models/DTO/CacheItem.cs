using MessagePack;

namespace MovieStoreB.Models.DTO
{
    [MessagePackObject]
    public abstract record CacheItem<T>
    {
        [Key(100)]
        public DateTime DateInserted { get; set; }

        public abstract T GetKey();
    }
}