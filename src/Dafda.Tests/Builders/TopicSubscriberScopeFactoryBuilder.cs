using Dafda.Consuming;
using Dafda.Tests.TestDoubles;

namespace Dafda.Tests.Builders
{
    internal class TopicSubscriberScopeFactoryBuilder
    {
        private MessageResult _messageResult = new MessageResultBuilder().Build();

        public TopicSubscriberScopeFactoryBuilder()
        {
        }
        
        public TopicSubscriberScopeFactoryBuilder WithMessageResult(MessageResult messageResult)
        {
            _messageResult = messageResult;
            return this;
        }

        public TopicSubscriberScopeFactoryBuilder WithMessageResult(MessageResultBuilder builder)
        {
            return WithMessageResult(builder.Build());
        }

        public ITopicSubscriberScopeFactory Build()
        {
            return new TopicSubscriberScopeFactoryStub(_messageResult);
        }
    }
}