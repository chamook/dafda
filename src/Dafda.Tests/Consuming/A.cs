using Dafda.Configuration;
using Dafda.Consuming;
using Dafda.Tests.Builders;

namespace Dafda.Tests.Consuming
{
    internal static class A
    {
        public static MessageResultBuilder MessageResult => new MessageResultBuilder();
        public static MessageHandlerRegistryBuilder MessageHandlerRegistry => new MessageHandlerRegistryBuilder();
        public static TopicSubscriberScopeFactoryBuilder TopicSubscriberScopeFactory => new TopicSubscriberScopeFactoryBuilder();

        public static ConsumerBuilder WithTopicSubscriberScopeFactory(this ConsumerBuilder consumerBuilder, TopicSubscriberScopeFactoryBuilder builder)
        {
            return consumerBuilder.WithTopicSubscriberScopeFactory(builder.Build());
        }
        
        public static ConsumerBuilder WithConfiguration(this ConsumerBuilder consumerBuilder, ConsumerConfigurationBuilder builder)
        {
            return consumerBuilder.WithConfiguration(builder.Build());
        }

    }
}