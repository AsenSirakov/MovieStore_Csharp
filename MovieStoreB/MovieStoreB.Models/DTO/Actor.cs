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

        // Parameterless constructor required for MessagePack
        public Actor() { }

        public Actor(string id, string name)
        {
            Id = id;
            Name = name;
        }

        // Don't serialize this method
        public override string GetKey() => Id;
    }
}