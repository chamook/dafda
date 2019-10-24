using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Outbox;
using Dafda.Producing.Kafka;

namespace Dafda.Producing
{
    public class Producer : IProducer
    {
        public static Builder Create()
        {
            return new Builder();
        }

        private readonly IKafkaProducer _kafkaProducer;
        private readonly OutgoingMessageFactory _outgoingMessageFactory;

        private Producer(IKafkaProducer kafkaProducer, IOutgoingMessageRegistry outgoingMessageRegistry, MessageIdGenerator messageIdGenerator)
        {
            _kafkaProducer = kafkaProducer;
            _outgoingMessageFactory = new OutgoingMessageFactory(outgoingMessageRegistry, messageIdGenerator);
        }

        public async Task Produce(object message)
        {
            var outgoingMessage = AssembleOutgoingMessage(message);

            await _kafkaProducer.Produce(outgoingMessage);
        }

        private OutgoingMessage AssembleOutgoingMessage(object message)
        {
            if (message is OutboxMessage outboxMessage)
            {
                return OutgoingMessage.Create()
                    .WithTopic(outboxMessage.Topic)
                    .WithMessageId(outboxMessage.MessageId.ToString())
                    .WithKey(outboxMessage.Key)
                    .WithValue(outboxMessage.Data)
                    .WithType(outboxMessage.Type)
                    .Build();
            }

            return _outgoingMessageFactory.Create(message);
        }

        public void Dispose()
        {
            _kafkaProducer?.Dispose();
        }

        #region Builder

        public class Builder
        {
            private IConfiguration _configuration = new Configuration.Configuration();
            private MessageIdGenerator _messageIdGenerator = MessageIdGenerator.Default;
            private IOutgoingMessageRegistry _outgoingMessageRegistry = new OutgoingMessageRegistry();
            private IKafkaProducerFactory _kafkaProducerFactory = new KafkaProducerFactory();

            internal Builder()
            {
            }

            public Builder WithConfiguration(IConfiguration configuration)
            {
                _configuration = configuration;
                return this;
            }

            public Builder WithMessageIdGenerator(MessageIdGenerator messageIdGenerator)
            {
                _messageIdGenerator = messageIdGenerator;
                return this;
            }

            public Builder WithOutgoingMessageRegistry(IOutgoingMessageRegistry outgoingMessageRegistry)
            {
                _outgoingMessageRegistry = outgoingMessageRegistry;
                return this;
            }

            public Builder WithKafkaProducerFactory(IKafkaProducerFactory kafkaProducerFactory)
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

        #endregion
    }
}