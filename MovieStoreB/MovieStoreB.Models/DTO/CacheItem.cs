namespace MovieStoreB.Models.DTO
{
    // Remove MessagePack attributes from abstract base class
    public abstract partial record CacheItem<T>
    {
        public virtual DateTime DateInserted { get; set; }

        public abstract T GetKey();
    }
}