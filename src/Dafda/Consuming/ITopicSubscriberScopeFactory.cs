using System.Collections.Generic;
using Dafda.Configuration;

namespace Dafda.Consuming
{
    public interface ITopicSubscriberScopeFactory
    {
        TopicSubscriberScope CreateTopicSubscriberScope(IConfiguration configuration, IEnumerable<string> subscribedTopics);
    }
}