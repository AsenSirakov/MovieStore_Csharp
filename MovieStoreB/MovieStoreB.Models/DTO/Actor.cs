namespace MovieStoreB.Models.DTO
{
    public record Actor : CacheItem<string>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Actor() { }
        public Actor(string id, string name) { Id = id; Name = name; }
        public override string GetKey() => Id;
    }
}