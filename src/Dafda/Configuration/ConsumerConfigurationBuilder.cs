using System.Collections.Generic;

namespace Dafda.Configuration
{
    public class ConsumerConfigurationBuilder : ConfigurationBuilderBase<ConsumerConfigurationBuilder>
    {
        private static readonly string[] RequiredConsumerConfigurationKeys =
        {
            ConfigurationKey.GroupId,
            ConfigurationKey.BootstrapServers
        };

        public override IEnumerable<string> ConfigurationKeys => ConfigurationKey.GetAllConsumerKeys();
        public override IEnumerable<string> RequiredConfigurationKeys => RequiredConsumerConfigurationKeys;

        public ConsumerConfigurationBuilder WithGroupId(string groupId)
        {
            return WithConfiguration(ConfigurationKey.GroupId, groupId);
        }
    }
}