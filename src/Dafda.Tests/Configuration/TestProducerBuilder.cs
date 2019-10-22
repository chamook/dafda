using Dafda.Configuration;
using Dafda.Producing;
using Xunit;

namespace Dafda.Tests.Configuration
{
    public class TestProducerBuilder
    {
        [Fact]
        public void Has_expected_message_id_generator()
        {
            var dummy = MessageIdGenerator.Default;

            var sut = new ProducerBuilder();
            sut.WithConfiguration(x => x.WithBootstrapServers("foo"));
            sut.WithMessageIdGenerator(dummy);
            var producerConfiguration = sut.Build();

            Assert.Equal(dummy, producerConfiguration.MessageIdGenerator);
        }

        [Fact]
        public void Has_expected_outgoing_message_registry()
        {
            var dummy = new OutgoingMessageRegistry();

            var sut = new ProducerBuilder();
            sut.WithConfiguration(x => x.WithBootstrapServers("foo"));
            sut.WithOutgoingMessageRegistry(dummy);
            var producerConfiguration = sut.Build();

            Assert.Equal(dummy, producerConfiguration.OutgoingMessageRegistry);
        }
    }
}