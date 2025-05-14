using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using MovieStoreB.Models.DTO;
using MovieStoreB.Models.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieStoreB.DL.Kafka
{

    internal class KafkaConsumer<TData,TKey> : BackgroundService, IKafkaConsumer<TData,TKey> where TData : CacheItem<TKey> where TKey : notnull
    {\
        private readonly ConsumerConfig _config;
        private readonly IConsumer<TKey, TData> _consumer;

        public KafkaConsumer()
        {
            _config = new ConsumerConfig()
            {
                BootstrapServers = "kafka-193981-0.cloudclusters.net:10300",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "admin",
                SaslPassword = "CPxpKSRD",
                EnableSslCertificateVerification = false
            };

            _consumer = new ConsumerBuilder<TKey, TData>(_config)
                .SetValueDeserializer(new MessagePackDeserializer<TData>())
                .Build();
            _consumer.Subscribe("test");
        }

        public async Task Consume(IEnumerable<TData> message)
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }
}
