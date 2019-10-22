using System.Collections.Generic;
using Dafda.Consuming;
using Dafda.Messaging;

namespace Dafda.Configuration
{
    public interface IConsumerConfiguration
    {
        IConfiguration Configuration { get; }

        IMessageHandlerRegistry MessageHandlerRegistry { get; }
        IHandlerUnitOfWorkFactory UnitOfWorkFactory { get; }
        
        ITopicSubscriberScopeFactory TopicSubscriberScopeFactory { get; }
        
        IEnumerable<string> SubscribedTopics { get; }
    }
}