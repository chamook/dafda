using System;
using Dafda.Consuming;
using Dafda.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Dafda.Configuration
{
    public interface IConsumerOptions
    {
        void WithConfigurationSource(ConfigurationSource configurationSource);
        void WithConfigurationSource(Microsoft.Extensions.Configuration.IConfiguration configuration);
        void WithNamingConvention(NamingConvention namingConvention);
        void WithEnvironmentStyle(string prefix = null, params string[] additionalPrefixes);
        void WithConfiguration(string key, string value);
        void WithGroupId(string groupId);
        void WithBootstrapServers(string bootstrapServers);
        void WithUnitOfWorkFactory<T>() where T : class, IHandlerUnitOfWorkFactory;
        void WithUnitOfWorkFactory(Func<IServiceProvider, IHandlerUnitOfWorkFactory> implementationFactory);
        void WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory);

        void RegisterMessageHandler<TMessage, TMessageHandler>(string topic, string messageType)
            where TMessage : class, new()
            where TMessageHandler : class, IMessageHandler<TMessage>;
    }

    internal class ConsumerOptions : IConsumerOptions
    {
        private readonly ConsumerConfigurationBuilder _configurationBuilder = new ConsumerConfigurationBuilder();
        private readonly ConsumerBuilder _consumerBuilder = new ConsumerBuilder();
        private readonly IMessageHandlerRegistry _messageHandlerRegistry = new MessageHandlerRegistry();

        private readonly IServiceCollection _services;

        public ConsumerOptions(IServiceCollection services)
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

        public void WithGroupId(string groupId)
        {
            _configurationBuilder.WithGroupId(groupId);
        }

        public void WithBootstrapServers(string bootstrapServers)
        {
            _configurationBuilder.WithBootstrapServers(bootstrapServers);
        }

        public void WithUnitOfWorkFactory<T>() where T : class, IHandlerUnitOfWorkFactory
        {
            _services.AddTransient<IHandlerUnitOfWorkFactory, T>();
        }

        public void WithUnitOfWorkFactory(Func<IServiceProvider, IHandlerUnitOfWorkFactory> implementationFactory)
        {
            _services.AddTransient(implementationFactory);
        }

        public void WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory)
        {
            _consumerBuilder.WithTopicSubscriberScopeFactory(topicSubscriberScopeFactory);
        }

        public void RegisterMessageHandler<TMessage, TMessageHandler>(string topic, string messageType)
            where TMessage : class, new()
            where TMessageHandler : class, IMessageHandler<TMessage>
        {
            _messageHandlerRegistry.Register<TMessage, TMessageHandler>(topic, messageType);
            _services.AddTransient<TMessageHandler>();
        }

        public Consumer Build(IServiceProvider provider)
        {
            var configuration = _configurationBuilder.Build();

            return _consumerBuilder
                .WithConfiguration(configuration)
                .WithMessageHandlerRegistry(_messageHandlerRegistry)
                .WithUnitOfWorkFactory(provider.GetRequiredService<IHandlerUnitOfWorkFactory>())
                .Build();
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