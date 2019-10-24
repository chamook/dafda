using System;
using Dafda.Outbox;
using Dafda.Producing;
using Microsoft.Extensions.DependencyInjection;

namespace Dafda.Configuration
{
    public interface IProducerOptions
    {
        void WithConfigurationSource(ConfigurationSource configurationSource);
        void WithConfigurationSource(Microsoft.Extensions.Configuration.IConfiguration configuration);
        void WithNamingConvention(NamingConvention namingConvention);
        void WithEnvironmentStyle(string prefix = null, params string[] additionalPrefixes);
        void WithConfiguration(string key, string value);
        void WithBootstrapServers(string bootstrapServers);
        void WithKafkaProducerFactory(IKafkaProducerFactory kafkaProducerFactory);
        void WithMessageIdGenerator(MessageIdGenerator messageIdGenerator);
        void Register<T>(string topic, string type, Func<T, string> keySelector) where T : class;
        void AddOutbox(Action<IOutboxOptions> config);
    }

    internal class ProducerOptions : IProducerOptions
    {
        private readonly ProducerConfigurationBuilder _configurationBuilder = new ProducerConfigurationBuilder();
        private readonly ProducerBuilder _builder = new ProducerBuilder();
        private readonly IOutgoingMessageRegistry _outgoingMessageRegistry = new OutgoingMessageRegistry();
        private readonly OutboxMessageCollector.Builder _outboxBuilder = OutboxMessageCollector.Create();

        private readonly IServiceCollection _services;

        public ProducerOptions(IServiceCollection services)
        {
            _services = services;
        }

        public void WithConfigurationSource(ConfigurationSource configurationSource)
        {
            _configurationBuilder.WithConfigurationSource(configurationSource);
        }

        public void WithConfigurationSource(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configurationBuilder.WithConfigurationSource(new DefaultConfigurationSource(configuration));
        }

        public void WithNamingConvention(NamingConvention namingConvention)
        {
            _configurationBuilder.WithNamingConvention(namingConvention);
        }

        public void WithEnvironmentStyle(string prefix = null, params string[] additionalPrefixes)
        {
            _configurationBuilder.WithEnvironmentStyle(prefix, additionalPrefixes);
        }

        public void WithConfiguration(string key, string value)
        {
            _configurationBuilder.WithConfiguration(key, value);
        }

        public void WithBootstrapServers(string bootstrapServers)
        {
            _configurationBuilder.WithBootstrapServers(bootstrapServers);
        }

        public void WithKafkaProducerFactory(IKafkaProducerFactory kafkaProducerFactory)
        {
            _builder.WithKafkaProducerFactory(kafkaProducerFactory);
        }

        public void WithMessageIdGenerator(MessageIdGenerator messageIdGenerator)
        {
            _builder.WithMessageIdGenerator(messageIdGenerator);
            _outboxBuilder.WithMessageIdGenerator(messageIdGenerator);
        }

        public void Register<T>(string topic, string type, Func<T, string> keySelector) where T : class
        {
            _outgoingMessageRegistry.Register(topic, type, keySelector);
        }

        public void AddOutbox(Action<IOutboxOptions> config)
        {
            var configuration = new OutboxOptions(_services);
            config?.Invoke(configuration);

            _services.AddTransient<IOutbox>(provider =>
            {
                var repository = provider.GetRequiredService<IOutboxMessageRepository>();

                return _outboxBuilder
                    .WithOutboxMessageRegistry(_outgoingMessageRegistry)
                    .WithOutboxMessageRepository(repository)
                    .Build();
            });
        }

        public IProducer Build()
        {
            var configuration = _configurationBuilder.Build();

            _builder.WithConfiguration(configuration);
            _builder.WithOutgoingMessageRegistry(_outgoingMessageRegistry);

            return _builder.Build();
        }

        private class DefaultConfigurationSource : ConfigurationSource
        {
            private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

            public DefaultConfigurationSource(Microsoft.Extensions.Configuration.IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public override string GetByKey(string keyName)
            {
                return _configuration[keyName];
            }
        }
    }
}