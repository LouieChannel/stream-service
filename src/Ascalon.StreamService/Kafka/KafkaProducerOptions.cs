using Confluent.Kafka;

namespace Ascalon.StreamService.Kafka
{
    /// <summary>
    /// Информация о настройке Producer в Kafka.
    /// </summary>
    public class KafkaProducerOptions
    {
        public ProducerConfig Config { get; set; }
    }
}
