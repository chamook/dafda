using Dafda.Configuration;
using Dafda.Consuming.Kafka;
using Dafda.Messaging;

namespace Dafda.Consuming
{
    public class ConsumerBuilder
    {
        private IConfiguration _configuration = new Configuration.Configuration();
        private ITopicSubscriberScopeFactory _topicSubscriberScopeFactory = new KafkaBasedTopicSubscriberScopeFactory();
        private IMessageHandlerRegistry _messageHandlerRegistry = new MessageHandlerRegistry();

        private IHandlerUnitOfWorkFactory _unitOfWorkFactory;

        public ConsumerBuilder WithConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }

        public ConsumerBuilder WithUnitOfWorkFactory(IHandlerUnitOfWorkFactory factory)
        {
            _unitOfWorkFactory = factory;
            return this;
        }

        public ConsumerBuilder WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory)
        {
            _topicSubscriberScopeFactory = topicSubscriberScopeFactory;
            return this;
        }

        public ConsumerBuilder WithMessageHandlerRegistry(IMessageHandlerRegistry messageHandlerRegistry)
        {
            _messageHandlerRegistry = messageHandlerRegistry;
            return this;
        }

        public Consumer Build()
        {
            return new Consumer(
                _configuration,
                _topicSubscriberScopeFactory,
                _messageHandlerRegistry,
                _unitOfWorkFactory,
                _messageHandlerRegistry.GetAllSubscribedTopics()
            );
        }
    }
}