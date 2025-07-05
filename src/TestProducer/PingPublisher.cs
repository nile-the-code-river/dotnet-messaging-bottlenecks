namespace TestProducer
{
    public class PingPublisher(ILogger<PingPublisher> _logger) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();

                var keyPressed = Console.ReadKey(true);
                if (keyPressed.Key != ConsoleKey.Escape)
                {
                    _logger.LogInformation("Pressed {Button}", keyPressed.Key.ToString());
                }

                await Task.Delay(200);
            }
        }
    }
}
