using Dafda.Producing;
using Dafda.Producing.Kafka;

namespace Dafda.Configuration
{
    public class ProducerBuilder
    {
        private IConfiguration _configuration = new Configuration();
        private MessageIdGenerator _messageIdGenerator = MessageIdGenerator.Default;
        private IOutgoingMessageRegistry _outgoingMessageRegistry = new OutgoingMessageRegistry();
        private IKafkaProducerFactory _kafkaProducerFactory = new KafkaProducerFactory();

        public ProducerBuilder WithConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }

        public ProducerBuilder WithMessageIdGenerator(MessageIdGenerator messageIdGenerator)
        {
            _messageIdGenerator = messageIdGenerator;
            return this;
        }

        public ProducerBuilder WithOutgoingMessageRegistry(IOutgoingMessageRegistry outgoingMessageRegistry)
        {
            _outgoingMessageRegistry = outgoingMessageRegistry;
            return this;
        }

        public ProducerBuilder WithKafkaProducerFactory(IKafkaProducerFactory kafkaProducerFactory)
        {
            _kafkaProducerFactory = kafkaProducerFactory;
            return this;
        }

        public IProducer Build()
        {
            var kafkaProducer = _kafkaProducerFactory.CreateProducer(_configuration);

            return new Producer(kafkaProducer, _outgoingMessageRegistry, _messageIdGenerator);
        }
    }
}