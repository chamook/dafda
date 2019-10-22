using Dafda.Tests.TestDoubles;
using Dafda.Configuration;
using Xunit;
using static Dafda.Tests.Configuration.TestHelper;

namespace Dafda.Tests.Configuration
{
    public class TestProducerConfigurationBuilder
    {
        [Fact]
        public void Test_producer_keys()
        {
            Assert.Equal(ConfigurationKey.GetAllProducerKeys(), new ProducerConfigurationBuilder().ConfigurationKeys);
        }

        [Fact]
        public void Test_required_producer_keys()
        {
            Assert.Equal(new string[]
            {
                ConfigurationKey.BootstrapServers,
            }, new ProducerConfigurationBuilder().RequiredConfigurationKeys);
        }

        // producer

        [Fact]
        public void _Can_validate_configuration()
        {
            var sut = new ProducerConfigurationBuilder();

            Assert.Throws<InvalidConfigurationException>(() => sut.Build());
        }

        [Fact]
        public void _Can_build_minimal_configuration()
        {
            var configuration = new ProducerConfigurationBuilder()
                .WithBootstrapServers("bar")
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void _Can_ignore_out_of_scope_values_from_configuration_source()
        {
            var configuration = new ProducerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: ConfigurationKey.BootstrapServers, value: "bar"),
                        (key: "dummy", value: "baz")
                    ))
                .Build();

            Assert.DoesNotContain(KeyValue("dummy", "baz"), configuration);
        }

        [Fact]
        public void _Can_use_configuration_value_from_source()
        {
            var configuration = new ProducerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: ConfigurationKey.BootstrapServers, value: "bar")
                    ))
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void _Can_use_configuration_value_from_source_with_environment_naming_convention()
        {
            var configuration = new ProducerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: "BOOTSTRAP_SERVERS", value: "bar")))
                .WithEnvironmentStyle()
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void _Can_use_configuration_value_from_source_with_environment_naming_convention_and_prefix()
        {
            var configuration = new ProducerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: "DEFAULT_KAFKA_BOOTSTRAP_SERVERS", value: "bar")
                    ))
                .WithEnvironmentStyle("DEFAULT_KAFKA")
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void _Can_overwrite_values_from_source()
        {
            var configuration = new ProducerConfigurationBuilder()
                .WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: ConfigurationKey.BootstrapServers, value: "foo")
                    ))
                .WithConfiguration(ConfigurationKey.BootstrapServers, "bar")
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "bar"), configuration);
        }

        [Fact]
        public void _Only_take_value_from_first_source_that_matches()
        {
            var configuration = new ProducerConfigurationBuilder().WithConfigurationSource(
                    new ConfigurationSourceStub(
                        (key: ConfigurationKey.BootstrapServers, value: "foo"),
                        (key: "BOOTSTRAP_SERVERS", value: "bar")
                    ))
                .WithNamingConvention(NamingConvention.Default)
                .WithEnvironmentStyle()
                .Build();

            Assert.Contains(expected: KeyValue(ConfigurationKey.BootstrapServers, "foo"), configuration);
        }
    }
}