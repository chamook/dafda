using Dafda.Configuration;
using Dafda.Tests.TestDoubles;
using Xunit;
using static Dafda.Tests.Configuration.TestHelper;

namespace Dafda.Tests.Configuration
{
    public class TestConsumerConfigurationBuilder
    {
        [Fact]
        public void Test_consumer_keys()
        {
            Assert.Equal(ConfigurationKey.GetAllConsumerKeys(), new ConsumerConfigurationBuilder().ConfigurationKeys);
        }

        [Fact]
        public void Test_required_consumer_keys()
        {
            Assert.Equal(new string[]
            {
                ConfigurationKey.GroupId,
                ConfigurationKey.BootstrapServers,
            }, new ConsumerConfigurationBuilder().RequiredConfigurationKeys);
        }

        [Fact]
        public void Can_validate_configuration()
        {
            var sut = new ConsumerConfigurationBuilder();

            var exception = Assert.Throws<InvalidConfigurationException>(() => sut.Build());

            Assert.Equal("Expected key 'group.id' not supplied in 'NullConfigurationSource' (attempted keys: 'group.id')", exception.Message);
        }

        [Fact]
        public void Can_build_minimal_configuration()
        {
            var configuration = new ConsumerConfigurationBuilder()
                .WithGroupId("foo")
                .WithBootstrapServers("bar")
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.GroupId, "foo"), configuration);
            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void Can_ignore_out_of_scope_values_from_configuration_source()
        {
            var configuration = new ConsumerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: ConfigurationKey.GroupId, value: "foo"),
                        (key: ConfigurationKey.BootstrapServers, value: "bar"),
                        (key: "dummy", value: "baz")
                    ))
                .Build();

            Assert.DoesNotContain(KeyValue("dummy", "baz"), configuration);
        }

        [Fact]
        public void Can_use_configuration_value_from_source()
        {
            var configuration = new ConsumerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: ConfigurationKey.GroupId, value: "foo"),
                        (key: ConfigurationKey.BootstrapServers, value: "bar")
                    ))
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.GroupId, "foo"), configuration);
            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void Can_use_configuration_value_from_source_with_environment_naming_convention()
        {
            var configuration = new ConsumerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: "GROUP_ID", value: "foo"),
                        (key: "BOOTSTRAP_SERVERS", value: "bar")
                    ))
                .WithEnvironmentStyle()
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.GroupId, "foo"), configuration);
            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void Can_use_configuration_value_from_source_with_environment_naming_convention_and_prefix()
        {
            var configuration = new ConsumerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: "DEFAULT_KAFKA_GROUP_ID", value: "foo"),
                        (key: "DEFAULT_KAFKA_BOOTSTRAP_SERVERS", value: "bar")
                    ))
                .WithEnvironmentStyle("DEFAULT_KAFKA")
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.GroupId, "foo"), configuration);
            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void Can_overwrite_values_from_source()
        {
            var configuration = new ConsumerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: ConfigurationKey.GroupId, value: "foo"),
                        (key: ConfigurationKey.BootstrapServers, value: "bar")
                    ))
                .WithConfiguration(ConfigurationKey.GroupId, "baz")
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.GroupId, "baz"), configuration);
        }

        [Fact]
        public void Only_take_value_from_first_source_that_matches()
        {
            var sut = new ConsumerConfigurationBuilder()
                .WithConfigurationSource(new ConfigurationSourceStub(
                    (key: ConfigurationKey.GroupId, value: "foo"),
                    (key: ConfigurationKey.BootstrapServers, value: "bar"),
                    (key: "GROUP_ID", value: "baz")
                ))
                .WithNamingConvention(NamingConvention.Default)
                .WithEnvironmentStyle();

            var configuration = sut.Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.GroupId, "foo"), configuration);
        }
    }
}