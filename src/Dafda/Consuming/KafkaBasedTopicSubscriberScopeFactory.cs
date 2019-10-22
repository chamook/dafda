using Confluent.Kafka;
using Dafda.Configuration;

namespace Dafda.Consuming
{
    public class KafkaBasedTopicSubscriberScopeFactory : ITopicSubscriberScopeFactory
    {
        public TopicSubscriberScope CreateTopicSubscriberScope(IConsumerConfiguration configuration)
        {
            var consumer = new ConsumerBuilder<string, string>(configuration.Configuration).Build();
            consumer.Subscribe(configuration.SubscribedTopics);

            return new KafkaConsumerScope(consumer);
        }
    }
}