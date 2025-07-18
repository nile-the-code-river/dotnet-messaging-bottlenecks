using Core.Models;
using MassTransit;

namespace Baseline.Consumer
{
    internal class SampleConsumer(ILogger<SampleConsumer> _logger) : IConsumer<SampleMessage>
    {
        public async Task Consume(ConsumeContext<SampleMessage> context)
        {
            _logger.LogInformation(context.Message.PaddingData.ToString());
        }
    }
}
