using System;
using Dafda.Producing;
using Dafda.Producing.Kafka;

namespace Dafda.Configuration
{
    public class ProducerBuilder
    {
        private readonly ProducerConfigurationBuilder _configuration = new ProducerConfigurationBuilder();

        private MessageIdGenerator _messageIdGenerator = MessageIdGenerator.Default;
        private IOutgoingMessageRegistry _outgoingMessageRegistry = new OutgoingMessageRegistry();
        private IKafkaProducerFactory _kafkaProducerFactory = new KafkaProducerFactory();

        public void WithConfiguration(Action<ProducerConfigurationBuilder> configuration)
        {
            configuration(_configuration);
        }

        public void WithMessageIdGenerator(MessageIdGenerator messageIdGenerator)
        {
            _messageIdGenerator = messageIdGenerator;
        }

        public void WithOutgoingMessageRegistry(IOutgoingMessageRegistry outgoingMessageRegistry)
        {
            _outgoingMessageRegistry = outgoingMessageRegistry;
        }

        public void WithKafkaProducerFactory(IKafkaProducerFactory kafkaProducerFactory)
        {
            _kafkaProducerFactory = kafkaProducerFactory;
        }

        public IProducerConfiguration Build()
        {
            var configuration = _configuration.Build();

            return new ProducerConfiguration(
                configuration,
                _messageIdGenerator,
                _outgoingMessageRegistry,
                _kafkaProducerFactory
            );
        }

        private class ProducerConfiguration : IProducerConfiguration
        {
            public ProducerConfiguration(IConfiguration configuration, MessageIdGenerator messageIdGenerator, IOutgoingMessageRegistry outgoingMessageRegistry, IKafkaProducerFactory kafkaProducerFactory)
            {
                Configuration = configuration;
                MessageIdGenerator = messageIdGenerator;
                OutgoingMessageRegistry = outgoingMessageRegistry;
                KafkaProducerFactory = kafkaProducerFactory;
            }

            public IConfiguration Configuration { get; }
            public MessageIdGenerator MessageIdGenerator { get; }
            public IOutgoingMessageRegistry OutgoingMessageRegistry { get; }
            public IKafkaProducerFactory KafkaProducerFactory { get; }
        }
    }
}