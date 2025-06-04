using MessagePack;

namespace MovieStoreB.Models.DTO
{
    [MessagePackObject]
    public abstract record CacheItem<T>
    {
        [Key(100)] // Use high key to avoid conflicts
        public DateTime DateInserted { get; set; }

        // Don't serialize abstract method - implement in derived classes
        public abstract T GetKey();
    }
}
