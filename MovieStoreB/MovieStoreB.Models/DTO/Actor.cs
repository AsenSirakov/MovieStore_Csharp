using MessagePack;

namespace MovieStoreB.Models.DTO
{
    [MessagePackObject]
    public record Actor : CacheItem<string>
    {
        [Key(0)]
        public string Id { get; set; } = string.Empty;

        [Key(1)]
        public string Name { get; set; } = string.Empty;

        [Key(2)]
        public override DateTime DateInserted { get; set; }

        public Actor() { }

        public Actor(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string GetKey() => Id;
    }
}