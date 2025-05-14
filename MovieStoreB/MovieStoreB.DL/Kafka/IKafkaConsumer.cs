namespace MovieStoreB.DL.Kafka
{
    internal interface IKafkaConsumer<TData, TKey>
    {
        Task Consume(IEnumerable<TData> messages);

    }
}