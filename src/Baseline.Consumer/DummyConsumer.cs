using Core.Models;
using MassTransit;

namespace Baseline.Consumer
{
    // must be fucking 'public' to consume
    //public class DummyConsumer(ILogger<DummyConsumer> _logger) : IConsumer<DummyMessage>
    //{
    //    public async Task Consume(ConsumeContext<DummyMessage> context)
    //        => _logger.LogInformation(context.Message.Value);
    //}
}
