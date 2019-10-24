using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Consuming;

namespace Dafda.Tests.TestDoubles
{
    public class TopicSubscriberScopeFactoryStub : ITopicSubscriberScopeFactory
    {
        private readonly TopicSubscriberScope _result;

        public TopicSubscriberScopeFactoryStub(MessageResult messageResult)
        {
            _result = new TopicSubscriberScopeStub(messageResult);
        }

        public TopicSubscriberScope CreateTopicSubscriberScope(IConfiguration configuration, IEnumerable<string> subscribedTopics)
        {
            return _result;
        }

        private class TopicSubscriberScopeStub : TopicSubscriberScope
        {
            private readonly MessageResult _result;

            public TopicSubscriberScopeStub(MessageResult result)
            {
                _result = result;
            }

            public override Task<MessageResult> GetNext(CancellationToken cancellationToken)
            {
                return Task.FromResult(_result);
            }

            public override void Dispose()
            {
            }
        }
    }
}