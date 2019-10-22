using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Messaging;
using Xunit;

namespace Dafda.Tests.Configuration
{
    public class TestConsumerBuilder
    {
        [Fact]
        public void Can_register_message_handler()
        {
            var sut = new ConsumerBuilder();
            sut.WithConfiguration(x => x
                .WithGroupId("foo")
                .WithBootstrapServers("bar")
            );
            sut.RegisterMessageHandler<DummyMessage, DummyMessageHandler>("dummyTopic", nameof(DummyMessage));

            var configuration = sut.Build();

            var registration = configuration.MessageHandlerRegistry.GetRegistrationFor(nameof(DummyMessage));

            Assert.Equal(typeof(DummyMessageHandler), registration.HandlerInstanceType);
        }

        public class DummyMessage
        {
        }

        // ReSharper disable once MemberCanBePrivate.Global
        private class DummyMessageHandler : IMessageHandler<DummyMessage>
        {
            public Task Handle(DummyMessage message)
            {
                LastHandledMessage = message;

                return Task.CompletedTask;
            }

            public object LastHandledMessage { get; private set; }
        }
    }
}