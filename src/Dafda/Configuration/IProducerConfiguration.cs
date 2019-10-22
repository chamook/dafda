using Dafda.Producing;

namespace Dafda.Configuration
{
    public interface IProducerConfiguration
    {
        Configuration Configuration { get; }
        MessageIdGenerator MessageIdGenerator { get; }
        IOutgoingMessageRegistry OutgoingMessageRegistry { get; }
        IKafkaProducerFactory KafkaProducerFactory { get; }
    }
}