using Dafda.Messaging;

namespace Dafda.Tests.Builders
{
    internal class MessageHandlerRegistryBuilder
    {
        private readonly MessageHandlerRegistry _registry = new MessageHandlerRegistry();

        public MessageHandlerRegistryBuilder Register<TMessage, THandler>(string topic, string messageType)
            where TMessage : class, new()
            where THandler : IMessageHandler<TMessage>
        {
            _registry.Register<TMessage, THandler>(topic, messageType);
            return this;
        }

        public MessageHandlerRegistry Build()
        {
            return _registry;
        }

        public static implicit operator MessageHandlerRegistry(MessageHandlerRegistryBuilder builder)
        {
            return builder.Build();
        }
    }
}