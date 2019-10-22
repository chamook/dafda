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
        void WithUnitOfWork<T>() where T : ScopedUnitOfWork;
        void WithUnitOfWork(Func<IServiceProvider, ScopedUnitOfWork> implementationFactory);
        void WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory);

        void RegisterMessageHandler<TMessage, TMessageHandler>(string topic, string messageType)
            where TMessage : class, new()
            where TMessageHandler : class, IMessageHandler<TMessage>;
    }

    internal class ConsumerOptions : IConsumerOptions
    {
        private readonly ConsumerBuilder _builder;
        private readonly IServiceCollection _services;

        public ConsumerOptions(ConsumerBuilder builder, IServiceCollection services)
        {
            _services = services;
            _builder = builder;
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

        public void WithGroupId(string groupId)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithGroupId(groupId));
        }

        public void WithBootstrapServers(string bootstrapServers)
        {
            _builder.WithConfiguration(_configurationBuilder => _configurationBuilder.WithBootstrapServers(bootstrapServers));
        }

        public void WithUnitOfWorkFactory<T>() where T : class, IHandlerUnitOfWorkFactory
        {
            _services.AddTransient<IHandlerUnitOfWorkFactory, T>();
        }

        public void WithUnitOfWorkFactory(Func<IServiceProvider, IHandlerUnitOfWorkFactory> implementationFactory)
        {
            _services.AddTransient(implementationFactory);
        }

        public void WithUnitOfWork<T>() where T : ScopedUnitOfWork
        {
            _services.AddTransient<ScopedUnitOfWork, T>();
        }

        public void WithUnitOfWork(Func<IServiceProvider, ScopedUnitOfWork> implementationFactory)
        {
            _services.AddTransient(implementationFactory);
        }

        public void WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory)
        {
            _builder.WithTopicSubscriberScopeFactory(topicSubscriberScopeFactory);
        }

        public void RegisterMessageHandler<TMessage, TMessageHandler>(string topic, string messageType)
            where TMessage : class, new()
            where TMessageHandler : class, IMessageHandler<TMessage>
        {
            _builder.RegisterMessageHandler<TMessage, TMessageHandler>(topic, messageType);
            _services.AddTransient<TMessageHandler>();
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