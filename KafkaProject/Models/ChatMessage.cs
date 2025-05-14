using MessagePack;

namespace KafkaProject.Models
{
    [MessagePackObject]
    public class ChatMessage
    {
        [Key(0)] public string Id { get; set; }
        [Key(1)] public string User { get; set; }
        [Key(2)] public string Message { get; set; }
        [Key(3)] public DateTime SendAt { get; set; }
    }
}
