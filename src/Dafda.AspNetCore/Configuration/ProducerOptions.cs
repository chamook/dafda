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
        private readonly ProducerBuilder _builder;
        private readonly IServiceCollection _services;
        private readonly IOutgoingMessageRegistry _outgoingMessageRegistry;

        public ProducerOptions(ProducerBuilder builder, IServiceCollection services, IOutgoingMessageRegistry outgoingMessageRegistry)
        {
            _builder = builder;
            _services = services;
            _outgoingMessageRegistry = outgoingMessageRegistry;
        }

        public void WithConfigurationSource(ConfigurationSource configurationSource)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithConfigurationSource(configurationSource));
        }

        public void WithConfigurationSource(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithConfigurationSource(new DefaultConfigurationSource(configuration)));
        }

        public void WithNamingConvention(NamingConvention namingConvention)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithNamingConvention(namingConvention));
        }

        public void WithEnvironmentStyle(string prefix = null, params string[] additionalPrefixes)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithEnvironmentStyle(prefix, additionalPrefixes));
        }

        public void WithConfiguration(string key, string value)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithConfiguration(key, value));
        }

        public void WithBootstrapServers(string bootstrapServers)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithBootstrapServers(bootstrapServers));
        }

        public void WithKafkaProducerFactory(IKafkaProducerFactory kafkaProducerFactory)
        {
            _builder.WithConfiguration(_configurationBuilder => _builder.WithKafkaProducerFactory(kafkaProducerFactory));
        }

        public void WithMessageIdGenerator(MessageIdGenerator messageIdGenerator)
        {
            _builder.WithMessageIdGenerator(messageIdGenerator);
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
                var producerConfiguration = provider.GetRequiredService<IProducerConfiguration>();
                var repository = provider.GetRequiredService<IOutboxMessageRepository>();
                return new OutboxMessageCollector(producerConfiguration.MessageIdGenerator, producerConfiguration.OutgoingMessageRegistry, repository);
            });
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