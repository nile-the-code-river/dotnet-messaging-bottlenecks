using Core.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baseline.Consumer
{
    // must be fucking 'public' to consume
    public class DummyConsumer(ILogger<DummyConsumer> _logger) : IConsumer<DummyMessage>
    {
        public async Task Consume(ConsumeContext<DummyMessage> context)
            => _logger.LogInformation(context.Message.Value);
    }
}
