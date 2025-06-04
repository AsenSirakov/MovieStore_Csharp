using MessagePack;

namespace MovieStoreB.Models.DTO
{
    [MessagePackObject]
    public record Movie : CacheItem<string>
    {
        [Key(0)]
        public string Id { get; set; } = string.Empty;

        [Key(1)]
        public string Title { get; set; } = string.Empty;

        [Key(2)]
        public int Year { get; set; }

        [Key(3)]
        public List<string> ActorIds { get; set; } = new List<string>();

       
        public override string GetKey() => Id;
    }
}