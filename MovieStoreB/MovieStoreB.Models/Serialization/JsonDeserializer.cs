using Confluent.Kafka;
using System.Text.Json;

namespace MovieStoreB.Models.Serialization
{
    public class JsonDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull) return default(T);
            var json = System.Text.Encoding.UTF8.GetString(data.ToArray());
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}
