using Core.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baseline.Consumer
{
    internal class PingConsumer(ILogger<PingConsumer> _logger) : IConsumer<Ping>
    {
        public Task Consume(ConsumeContext<Ping> context)
        {
            var button = context.Message.button;
            _logger.LogInformation("Button pressed {button}", button);

            return Task.CompletedTask;
        }
    }
}
