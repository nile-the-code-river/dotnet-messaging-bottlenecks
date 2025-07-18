
using Core.Models;
using MassTransit;

namespace Baseline.Producer
{
    public class SampleProducer : BackgroundService
    {
        private readonly ILogger<SampleProducer> _logger;
        private readonly IBus _bus;

        private readonly string _smSampleMsg;
        private readonly string _mdSampleMsg;
        private readonly string _lgSampleMsg;

        public SampleProducer(ILogger<SampleProducer> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;

            _smSampleMsg = SampleMessage.LoadJsonData(MessageSize.Small);
            _mdSampleMsg = SampleMessage.LoadJsonData(MessageSize.Medium);
            _lgSampleMsg = SampleMessage.LoadJsonData(MessageSize.Large);
            _logger.LogWarning($"Sample message json datum loaded: ex.) {_smSampleMsg}");
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("======== Sample Producer ========");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("S(1), M(2), L(3)");
                var keyPressed = Console.ReadKey(true);

                switch (keyPressed.Key)
                {
                    case ConsoleKey.Escape:
                        _logger.LogInformation("======== Ended ========");
                        return;
                    case ConsoleKey.S:
                    case ConsoleKey.NumPad1:
                        _logger.LogInformation("____SMALL____");
                        _logger.LogWarning(_smSampleMsg);
                        await _bus.Publish(new SampleMessage());
                        break;
                    case ConsoleKey.M:
                    case ConsoleKey.NumPad2:
                        _logger.LogInformation("____MEDIUM____");
                        _logger.LogWarning(_mdSampleMsg);
                        break;
                    case ConsoleKey.L:
                    case ConsoleKey.NumPad3:
                        _logger.LogInformation("____LARGE____");
                        _logger.LogWarning(_lgSampleMsg);
                        break;
                }
            }
        }
    }
}
