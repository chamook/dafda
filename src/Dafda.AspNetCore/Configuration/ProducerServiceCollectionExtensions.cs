using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dafda.Configuration
{
    public static class ProducerServiceCollectionExtensions
    {
        public static void AddProducer(this IServiceCollection services, Action<IProducerOptions> options)
        {
            var consumerOptions = new ProducerOptions(services);
            options?.Invoke(consumerOptions);

            services.AddSingleton(provider => consumerOptions.Build());
        }
    }
}