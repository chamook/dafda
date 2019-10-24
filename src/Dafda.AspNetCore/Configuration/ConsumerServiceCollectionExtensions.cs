using System;
using Dafda.Consuming;
using Dafda.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Dafda.Configuration
{
    public static class ConsumerServiceCollectionExtensions
    {
        public static void AddConsumer(this IServiceCollection services, Action<IConsumerOptions> options = null)
        {
            var consumerOptions = new ConsumerOptions(services);
            consumerOptions.WithUnitOfWorkFactory<ServiceProviderUnitOfWorkFactory>();
            options?.Invoke(consumerOptions);

            services.AddTransient(provider => consumerOptions.Build(provider));

            services.AddHostedService<SubscriberHostedService>();
        }
    }
}