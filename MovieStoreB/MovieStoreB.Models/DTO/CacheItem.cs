using MessagePack;

namespace MovieStoreB.Models.DTO
{
    public abstract partial record CacheItem<T>
    {
        [IgnoreMember]
        public virtual DateTime DateInserted { get; set; }

        public abstract T GetKey();
    }
}