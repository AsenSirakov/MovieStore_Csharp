using KafkaChat;
using KafkaProject.Models;
using KafkaProject.Models;
using KafkaProject;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.Write("Enter your username: ");
        AppSetting.LocalUsername = Console.ReadLine();

        var producer = new KafkaProducer();
        var consumer = new KafkaConsumer();

        var cts = new CancellationTokenSource();
        _ = Task.Run(() => consumer.StartConsuming(cts.Token));

        Console.WriteLine("Start chatting! Type your messages below:");

        while (true)
        {
            Console.Write($"[{AppSetting.LocalUsername}]> ");
            var input = Console.ReadLine();

            var msg = new ChatMessage
            {
                Id = Guid.NewGuid().ToString(),
                User = AppSetting.LocalUsername,
                Message = input,
                SendAt = DateTime.Now

            };

            await producer.SendMessage(msg);
        }
    }
}
