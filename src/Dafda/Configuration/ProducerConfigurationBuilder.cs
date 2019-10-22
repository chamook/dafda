using System.Collections.Generic;

namespace Dafda.Configuration
{
    public class ProducerConfigurationBuilder : ConfigurationBuilderBase<ProducerConfigurationBuilder>
    {
        private static readonly string[] RequiredProducerConfigurationKeys =
        {
            ConfigurationKey.BootstrapServers
        };

        public override IEnumerable<string> ConfigurationKeys => ConfigurationKey.GetAllProducerKeys();
        public override IEnumerable<string> RequiredConfigurationKeys => RequiredProducerConfigurationKeys;
    }
}