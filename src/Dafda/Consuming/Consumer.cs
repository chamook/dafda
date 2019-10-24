using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Consuming.Kafka;
using Dafda.Messaging;

namespace Dafda.Consuming
{
    public class Consumer
    {
        public static Builder Create()
        {
            return new Builder();
        }

        private readonly IConfiguration _configuration;
        private readonly ITopicSubscriberScopeFactory _topicSubscriberScopeFactory;
        private readonly LocalMessageDispatcher _localMessageDispatcher;
        private readonly IList<string> _subscribedTopics;

        private Consumer(IConfiguration configuration, ITopicSubscriberScopeFactory subscriberScopeFactory, IMessageHandlerRegistry messageHandlerRegistry, IHandlerUnitOfWorkFactory unitOfWorkFactory, IEnumerable<string> subscribedTopics)
        {
            _localMessageDispatcher = new LocalMessageDispatcher(messageHandlerRegistry, unitOfWorkFactory);
            _topicSubscriberScopeFactory = subscriberScopeFactory;
            _configuration = configuration;
            _subscribedTopics = subscribedTopics.ToList();
        }

        public async Task ConsumeAll(CancellationToken cancellationToken)
        {
            using (var subscriberScope = _topicSubscriberScopeFactory.CreateTopicSubscriberScope(_configuration, _subscribedTopics))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await ProcessNextMessage(subscriberScope, cancellationToken);
                }
            }
        }

        public async Task ConsumeSingle(CancellationToken cancellationToken)
        {
            using (var subscriberScope = _topicSubscriberScopeFactory.CreateTopicSubscriberScope(_configuration, _subscribedTopics))
            {
                await ProcessNextMessage(subscriberScope, cancellationToken);
            }
        }

        private async Task ProcessNextMessage(TopicSubscriberScope topicSubscriberScope, CancellationToken cancellationToken)
        {
            var messageResult = await topicSubscriberScope.GetNext(cancellationToken);
            await _localMessageDispatcher.Dispatch(messageResult.Message);

            if (_configuration.EnableAutoCommit == false)
            {
                await messageResult.Commit();
            }
        }

        #region Builder

        public class Builder
        {
            private IConfiguration _configuration = new Configuration.Configuration();
            private ITopicSubscriberScopeFactory _topicSubscriberScopeFactory = new KafkaBasedTopicSubscriberScopeFactory();
            private IMessageHandlerRegistry _messageHandlerRegistry = new MessageHandlerRegistry();

            private IHandlerUnitOfWorkFactory _unitOfWorkFactory;

            internal Builder()
            {
            }

            public Builder WithConfiguration(IConfiguration configuration)
            {
                _configuration = configuration;
                return this;
            }

            public Builder WithUnitOfWorkFactory(IHandlerUnitOfWorkFactory factory)
            {
                _unitOfWorkFactory = factory;
                return this;
            }

            public Builder WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory)
            {
                _topicSubscriberScopeFactory = topicSubscriberScopeFactory;
                return this;
            }

            public Builder WithMessageHandlerRegistry(IMessageHandlerRegistry messageHandlerRegistry)
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

        #endregion
    }
}