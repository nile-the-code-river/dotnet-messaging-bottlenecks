using Core.Models;
using MassTransit;

namespace Baseline.Producer
{
    public class DummyProducer(ILogger<DummyProducer> _logger, IBus _bus) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("======== Producer ========");

            while (!stoppingToken.IsCancellationRequested)
            {
                var keyPressed = Console.ReadKey(true);

                switch (keyPressed.Key)
                {
                    case ConsoleKey.Escape:
                        _logger.LogInformation("======== Ended ========");
                        return;
                    default:
                        await _bus.Publish(new DummyMessage() { Value = "Test Message" });
                        _logger.LogInformation("Message successfully sent");
                        break;
                }
            }
        }
    }
}
