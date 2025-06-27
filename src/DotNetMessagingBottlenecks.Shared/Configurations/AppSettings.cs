using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Configurations
{
    internal class AppSettings
    {
        public RabbitMQSettings RabbitMQ { get; set; } = new();
        public PerformanceSettings Performance { get; set; } = new();
        public LoggingSettings Logging { get; set; } = new();
    }
}
