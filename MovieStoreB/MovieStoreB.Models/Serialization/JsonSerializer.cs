using Confluent.Kafka;
using System.Text.Json;

namespace MovieStoreB.Models.Serialization
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }
    }
}
