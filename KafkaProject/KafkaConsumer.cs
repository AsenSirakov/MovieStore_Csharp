using Confluent.Kafka;
using KafkaProject.Models;
using KafkaProject.Models;
using KafkaProject;
using System.Text.Json;
using MessagePack;

namespace KafkaChat
{
    public class KafkaConsumer
    {
        public void StartConsuming(CancellationToken token)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "kafka-193981-0.cloudclusters.net:10300",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "admin",
                SaslPassword = "CPxpKSRD",
                EnableSslCertificateVerification = false,
                GroupId = Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Latest
            };
           

            using var consumer = new ConsumerBuilder<Ignore, byte[]>(config).Build();
            consumer.Subscribe("test");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(token);
                    var message = MessagePackSerializer.Deserialize<ChatMessage>(result.Message.Value);

                    if (message.User != AppSetting.LocalUsername)
                    {

                        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");

                        // Print incoming message
                        Console.WriteLine($"[{message.SendAt:HH:mm:ss}] [{message.User}] {message.Message}");

                        // Reprint prompt line
                        Console.Write("> ");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($" Consume error: {ex.Error.Reason}");
                }
            }
        }
    }
}
