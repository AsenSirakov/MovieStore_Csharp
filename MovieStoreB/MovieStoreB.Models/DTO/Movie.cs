namespace MovieStoreB.Models.DTO
{
    public record Movie : CacheItem<string>
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public List<string> ActorIds { get; set; } = new List<string>();
        public override string GetKey() => Id;
    }
}