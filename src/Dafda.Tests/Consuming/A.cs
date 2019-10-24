using Dafda.Configuration;
using Dafda.Messaging;
using Dafda.Tests.Builders;

namespace Dafda.Tests.Consuming
{
    internal static class A
    {
        public static ConsumerConfigurationBuilder ValidConsumerConfiguration => new ConsumerConfigurationBuilder()
            .WithGroupId("foo.group.id")
            .WithBootstrapServers("foo.bootstrap.servers");

        public static MessageResultBuilder MessageResult => new MessageResultBuilder();
        public static MessageHandlerRegistryBuilder MessageHandlerRegistry => new MessageHandlerRegistryBuilder();
        public static ConsumerBuilder Consumer => new ConsumerBuilder();
    }
}