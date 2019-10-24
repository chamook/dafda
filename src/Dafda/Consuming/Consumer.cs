using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Messaging;

namespace Dafda.Consuming
{
    public class Consumer
    {
        private readonly IConfiguration _configuration;
        private readonly ITopicSubscriberScopeFactory _topicSubscriberScopeFactory;
        private readonly LocalMessageDispatcher _localMessageDispatcher;
        private readonly IList<string> _subscribedTopics;

        public Consumer(IConfiguration configuration, ITopicSubscriberScopeFactory subscriberScopeFactory, IMessageHandlerRegistry messageHandlerRegistry, IHandlerUnitOfWorkFactory unitOfWorkFactory, IEnumerable<string> subscribedTopics)
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
    }
}