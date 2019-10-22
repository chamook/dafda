using System;
using System.Collections.Generic;
using Dafda.Consuming;
using Dafda.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Dafda.Configuration
{
    public static class ConsumerServiceCollectionExtensions
    {
        public static void AddConsumer(this IServiceCollection services, Action<IConsumerOptions> options = null)
        {
            var configurationBuilder = new ConsumerBuilder();
            var consumerOptions = new ConsumerOptions(configurationBuilder, services);
            consumerOptions.WithUnitOfWorkFactory<ServiceProviderUnitOfWorkFactory>();
            consumerOptions.WithUnitOfWork<ScopedUnitOfWork>();
            options?.Invoke(consumerOptions);
            var configuration = configurationBuilder.Build();

            services.AddSingleton<IConsumerConfiguration>(provider => new ServiceProviderConsumerConfiguration(configuration, provider));
            services.AddTransient(provider => new Consumer(provider.GetRequiredService<IConsumerConfiguration>()));
            services.AddHostedService<SubscriberHostedService>();
        }

        private class ServiceProviderConsumerConfiguration : IConsumerConfiguration
        {
            private readonly IConsumerConfiguration _inner;
            private readonly IServiceProvider _provider;

            public ServiceProviderConsumerConfiguration(IConsumerConfiguration inner, IServiceProvider provider)
            {
                _inner = inner;
                _provider = provider;
            }

            public IConfiguration Configuration => _inner.Configuration;
            public IMessageHandlerRegistry MessageHandlerRegistry => _inner.MessageHandlerRegistry;
            public IHandlerUnitOfWorkFactory UnitOfWorkFactory => _provider.GetRequiredService<IHandlerUnitOfWorkFactory>();
            public ITopicSubscriberScopeFactory TopicSubscriberScopeFactory => _inner.TopicSubscriberScopeFactory;
            public IEnumerable<string> SubscribedTopics => _inner.SubscribedTopics;
        }
    }
}