using System;
using System.Collections.Generic;
using Dafda.Consuming;
using Dafda.Messaging;

namespace Dafda.Configuration
{
    public class ConsumerBuilder
    {
        private readonly ConsumerConfigurationBuilder _configuration = new ConsumerConfigurationBuilder();
        private readonly IMessageHandlerRegistry _messageHandlerRegistry = new MessageHandlerRegistry();

        private IHandlerUnitOfWorkFactory _unitOfWorkFactory;
        private ITopicSubscriberScopeFactory _topicSubscriberScopeFactory = new KafkaBasedTopicSubscriberScopeFactory();

        public void WithConfiguration(Action<ConsumerConfigurationBuilder> configuration)
        {
            configuration(_configuration);
        }

        public void WithUnitOfWorkFactory(IHandlerUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public void WithUnitOfWorkFactory(Func<Type, IHandlerUnitOfWork> factory)
        {
            _unitOfWorkFactory = new DefaultUnitOfWorkFactory(factory);
        }

        public void WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory)
        {
            _topicSubscriberScopeFactory = topicSubscriberScopeFactory;
        }

        public void RegisterMessageHandler<TMessage, TMessageHandler>(string topic, string messageType)
            where TMessage : class, new()
            where TMessageHandler : IMessageHandler<TMessage>
        {
            _messageHandlerRegistry.Register<TMessage, TMessageHandler>(topic, messageType);
        }

        public IConsumerConfiguration Build()
        {
            var configuration = _configuration.Build();

            return new ConsumerConfiguration(
                configuration: configuration,
                messageHandlerRegistry: _messageHandlerRegistry,
                unitOfWorkFactory: _unitOfWorkFactory,
                topicSubscriberScopeFactory: _topicSubscriberScopeFactory
            );
        }

        private class ConsumerConfiguration : IConsumerConfiguration
        {
            public ConsumerConfiguration(Configuration configuration, IMessageHandlerRegistry messageHandlerRegistry,
                IHandlerUnitOfWorkFactory unitOfWorkFactory, ITopicSubscriberScopeFactory topicSubscriberScopeFactory)
            {
                Configuration = configuration;
                MessageHandlerRegistry = messageHandlerRegistry;
                UnitOfWorkFactory = unitOfWorkFactory;
                TopicSubscriberScopeFactory = topicSubscriberScopeFactory;
            }

            public Configuration Configuration { get; }
            public IMessageHandlerRegistry MessageHandlerRegistry { get; }
            public IHandlerUnitOfWorkFactory UnitOfWorkFactory { get; }
            public ITopicSubscriberScopeFactory TopicSubscriberScopeFactory { get; }

            public bool EnableAutoCommit
            {
                get
                {
                    const bool defaultAutoCommitStrategy = true;

                    if (!Configuration.TryGetValue(ConfigurationKey.EnableAutoCommit, out var value))
                    {
                        return defaultAutoCommitStrategy;
                    }

                    return bool.Parse(value);
                }
            }

            public IEnumerable<string> SubscribedTopics => MessageHandlerRegistry.GetAllSubscribedTopics();
        }
    }
}