using System.Collections.Generic;
using Confluent.Kafka;
using Dafda.Configuration;

namespace Dafda.Consuming.Kafka
{
    public class KafkaBasedTopicSubscriberScopeFactory : ITopicSubscriberScopeFactory
    {
        public TopicSubscriberScope CreateTopicSubscriberScope(IConfiguration configuration, IEnumerable<string> subscribedTopics)
        {
            var consumer = new ConsumerBuilder<string, string>(configuration).Build();
            consumer.Subscribe(subscribedTopics);

            return new KafkaConsumerScope(consumer);
        }
    }
}