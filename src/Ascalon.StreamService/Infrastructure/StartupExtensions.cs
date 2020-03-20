using Ascalon.Kafka;
using Ascalon.StreamService.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ascalon.StreamService.Infrastructure
{
    public static class StartupExtensions
    {
        public static void AddKafkaProducer(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var producerConfigOptions = serviceProvider.GetService<IOptions<KafkaProducerOptions>>().Value;

                return new Producer(producerConfigOptions.Config!, serviceProvider.GetService<ILogger<Producer>>());
            });
        }
    }
}
