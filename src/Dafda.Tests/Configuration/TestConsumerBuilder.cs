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
            sut.WithConfiguration(x=>x
                    .WithGroupId("foo")
                    .WithBootstrapServers("bar")
            );
            sut.RegisterMessageHandler<DummyMessage, DummyMessageHandler>("dummyTopic", nameof(DummyMessage));

            var configuration = sut.Build();

            var registration = configuration.MessageHandlerRegistry.GetRegistrationFor(nameof(DummyMessage));

            Assert.Equal(typeof(DummyMessageHandler), registration.HandlerInstanceType);
        }

        [Fact]
        public void returns_expected_auto_commit_when_not_set()
        {
            var sut = new ConsumerBuilder();
            sut.WithConfiguration( x => x
                    .WithGroupId("foo")
                    .WithBootstrapServers("bar")
            );

            var configuration = sut.Build();

            Assert.True(configuration.EnableAutoCommit);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("TRUE", true)]
        [InlineData("false", false)]
        [InlineData("FALSE", false)]
        public void returns_expected_auto_commit_when_configured_with_valid_value(string configValue, bool expected)
        {
            var sut = new ConsumerBuilder();
            sut.WithConfiguration(x => x
                    .WithGroupId("foo")
                    .WithBootstrapServers("bar")
                    .WithConfiguration(ConfigurationKey.EnableAutoCommit, configValue)
            );

            var configuration = sut.Build();

            Assert.Equal(expected, configuration.EnableAutoCommit);
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