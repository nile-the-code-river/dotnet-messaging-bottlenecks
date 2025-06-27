using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Configurations
{
    internal class RabbitMQSettings
    {
        public const string SectionName = "RabbitMQ";

        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";

        // 성능 관련 설정
        public ushort PrefetchCount { get; set; } = 10;
        public int ConcurrencyLimit { get; set; } = Environment.ProcessorCount;
        public bool PublisherConfirms { get; set; } = true;
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

        // 큐 설정
        public bool DurableQueues { get; set; } = true;
        public bool AutoDeleteQueues { get; set; } = false;

        public string GetConnectionString()
        {
            return $"amqp://{Username}:{Password}@{Host}:{Port}{VirtualHost}";
        }
    }
}
