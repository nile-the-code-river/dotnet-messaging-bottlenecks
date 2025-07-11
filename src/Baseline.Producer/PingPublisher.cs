using Core.Events;
using MassTransit;

namespace Baseline.Producer
{
    internal class PingPublisher(ILogger<PingPublisher> _logger, IBus _bus) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();

                var keyPressed = Console.ReadKey(true);
                if(keyPressed.Key != ConsoleKey.Escape)
                {
                    _logger.LogInformation("[Producer] Pressed {button}", keyPressed.Key.ToString());
                    _bus.Publish(new Ping(keyPressed.Key.ToString()));
                }

                await Task.Delay(200);
            }
        }
    }
}
