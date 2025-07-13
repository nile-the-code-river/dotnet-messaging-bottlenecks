using Core.Models;
using Core.Monitoring;
using MassTransit;
using System.Drawing;

namespace Baseline.Producer
{
    public class OrderProducer : BackgroundService
    {
        private readonly ILogger<OrderProducer> _logger;
        private readonly IBus _bus;
        private readonly Random _random = new();
        private static PerformanceTracker? _tracker;

        public OrderProducer(ILogger<OrderProducer> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("=== Baseline Load Test Started ===");
            _logger.LogInformation("Press 'S' for stress test, 'N' for normal load, 'H' for high load, ESC to stop...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var keyPressed = Console.ReadKey(true);

                switch (keyPressed.Key)
                {
                    case ConsoleKey.S: // Stress Test
                        await RunStressTest(stoppingToken);
                        break;
                    case ConsoleKey.N: // Normal Load
                        await RunNormalLoad(stoppingToken);
                        break;
                    case ConsoleKey.H: // High Load  
                        await RunHighLoad(stoppingToken);
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private async Task RunStressTest(CancellationToken cancellationToken)
        {
            _tracker = new PerformanceTracker(); // 측정 시작
            _logger.LogInformation("🔥 STRESS TEST: 1000 msg/sec for 30 seconds");

            var endTime = DateTime.UtcNow.AddSeconds(30);
            var messageCount = 0;

            // 주기적 상태 출력을 위한 타이머
            using var timer = new Timer(_ => _tracker.LogCurrentStats(), null,
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            while (DateTime.UtcNow < endTime && !cancellationToken.IsCancellationRequested)
            {
                // 1초에 1000개 = 1ms마다 1개
                var tasks = new List<Task>();
                for (int i = 0; i < 10; i++) // 10개씩 배치 처리
                {
                    tasks.Add(ProduceOrderMessage());
                    messageCount++;
                }

                await Task.WhenAll(tasks);
                await Task.Delay(10, cancellationToken); // 10ms 대기 (100배치/초 = 1000개/초)

                if (messageCount % 1000 == 0)
                    _logger.LogInformation("📊 Sent {Count} messages", messageCount);
            }

            _logger.LogInformation("✅ Stress test completed. Generating final report...");
            await Task.Delay(5000); // Consumer가 처리할 시간
            _tracker.LogFinalReport();
        }

        private async Task RunNormalLoad(CancellationToken cancellationToken)
        {
            _logger.LogInformation("📈 NORMAL LOAD: 100 msg/sec for 5 minutes");

            var endTime = DateTime.UtcNow.AddMinutes(5);
            var messageCount = 0;

            while (DateTime.UtcNow < endTime && !cancellationToken.IsCancellationRequested)
            {
                await ProduceOrderMessage();
                messageCount++;

                if (messageCount % 100 == 0)
                    _logger.LogInformation("📊 Normal load: {Count} messages sent", messageCount);

                await Task.Delay(10, cancellationToken); // 100msg/sec
            }

            _logger.LogInformation("✅ Normal load completed. Total: {Count} messages", messageCount);
        }

        private async Task RunHighLoad(CancellationToken cancellationToken)
        {
            _logger.LogInformation("⚡ HIGH LOAD: 500 msg/sec for 2 minutes");

            var endTime = DateTime.UtcNow.AddMinutes(2);
            var messageCount = 0;

            while (DateTime.UtcNow < endTime && !cancellationToken.IsCancellationRequested)
            {
                var tasks = new List<Task>();
                for (int i = 0; i < 5; i++) // 5개씩 배치
                {
                    tasks.Add(ProduceOrderMessage());
                    messageCount++;
                }

                await Task.WhenAll(tasks);

                if (messageCount % 500 == 0)
                    _logger.LogInformation("📊 High load: {Count} messages sent", messageCount);

                await Task.Delay(10, cancellationToken); // 500msg/sec
            }

            _logger.LogInformation("✅ High load completed. Total: {Count} messages", messageCount);
        }

        public static void NotifyMessageProcessed(double processingTimeMs)
        {
            _tracker?.MessageProcessed(processingTimeMs);
            Console.WriteLine($"[DEBUG] Message processed notification received: {processingTimeMs:F2}ms");

        }

        // Tracker 상태 확인용 메서드 추가
        public static bool IsTrackerActive()
        {
            return _tracker != null;
        }
        public enum MessageSize { Small, Medium, Large, XLarge }

        private async Task ProduceOrderMessage(MessageSize size = MessageSize.Medium)
        {
            int itemCount = size switch
            {
                MessageSize.Small => _random.Next(1, 3),      // 1KB 메시지
                MessageSize.Medium => _random.Next(3, 8),     // 10KB 메시지  
                MessageSize.Large => _random.Next(10, 20),    // 100KB 메시지
                MessageSize.XLarge => _random.Next(50, 100),  // 1MB 메시지
                _ => _random.Next(3, 8)
            };

            var orderMessage = new OrderMessage
            {
                OrderId = Guid.NewGuid(),
                CustomerId = $"CUST_{_random.Next(1000, 9999)}",
                OrderDate = DateTime.UtcNow,
                Items = GenerateOrderItems(itemCount),
                CustomerInfo = GenerateCustomerInfo(),
                ShippingAddress = GenerateShippingAddress(),
                OrderNotes = GenerateOrderNotes() // size 파라미터 추가!
            };

            orderMessage.TotalAmount = orderMessage.Items.Sum(x => x.Price * x.Quantity);
            await _bus.Publish(orderMessage);

            // 측정 로직
            _tracker?.MessageSent();
        }

        private void LogGCStats()
        {
            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);
            var memory = GC.GetTotalMemory(false) / 1024 / 1024; // MB

            _logger.LogInformation("🧠 GC Stats - Gen0: {Gen0}, Gen1: {Gen1}, Gen2: {Gen2}, Memory: {Memory}MB",
                gen0, gen1, gen2, memory);
        }

        private List<OrderItemDto> GenerateOrderItems(int itemCount)
        {
            var items = new List<OrderItemDto>();

            for (int i = 0; i < itemCount; i++)
            {
                items.Add(new OrderItemDto
                {
                    ProductId = $"PROD_{_random.Next(100, 999)}",
                    ProductName = GenerateProductName(),
                    Quantity = _random.Next(1, 5),
                    Price = (decimal)(_random.NextDouble() * 100 + 10),
                    Category = GetRandomCategory(),
                    Description = GenerateProductDescription()
                });
            }

            return items;
        }

        private CustomerInfo GenerateCustomerInfo()
        {
            return new CustomerInfo // 매번 새 객체
            {
                Name = GenerateCustomerName(),
                Email = GenerateEmail(),
                Phone = GeneratePhoneNumber(),
                LoyaltyLevel = GetRandomLoyaltyLevel(),
                PreferredLanguage = "EN",
                AccountCreatedDate = DateTime.UtcNow.AddDays(-_random.Next(30, 365))
            };
        }

        private ShippingAddress GenerateShippingAddress()
        {
            return new ShippingAddress // 매번 새 객체
            {
                Street = GenerateStreetAddress(),
                City = GetRandomCity(),
                State = GetRandomState(),
                ZipCode = GenerateZipCode(),
                Country = "US",
                DeliveryInstructions = GenerateDeliveryInstructions()
            };
        }

        private string GenerateProductName()
        {
            // 문자열 연결로 새로운 문자열 생성 (일반적인 패턴)
            var adjectives = new[] { "Premium", "Deluxe", "Standard", "Economy", "Professional" };
            var products = new[] { "Laptop", "Mouse", "Keyboard", "Monitor", "Headphones" };

            return $"{adjectives[_random.Next(adjectives.Length)]} {products[_random.Next(products.Length)]} " +
                   $"Model-{_random.Next(100, 999)}";
        }

        private string GenerateProductDescription()
        {
            // StringBuilder 없이 문자열 연결 (일반적이지만 할당 많은 패턴)
            var description = "High-quality product with excellent features. ";
            description += $"Manufactured in {DateTime.UtcNow.Year}. ";
            description += "Comes with standard warranty and customer support. ";
            description += $"Product code: {Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}. ";
            description += "Perfect for both personal and professional use.";

            return description;
        }

        private string GenerateOrderNotes()
        {
            // 조건부 문자열 생성 (일반적인 비즈니스 로직)
            var notes = new List<string>(); // 새 리스트

            if (_random.NextDouble() > 0.7)
                notes.Add("Express delivery requested");

            if (_random.NextDouble() > 0.8)
                notes.Add("Gift wrapping required");

            if (_random.NextDouble() > 0.9)
                notes.Add("Fragile items - handle with care");

            return string.Join("; ", notes); // 문자열 조인
        }

        private string GenerateCustomerName()
        {
            var firstNames = new[] { "John", "Jane", "Mike", "Sarah", "David", "Lisa" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia" };

            return $"{firstNames[_random.Next(firstNames.Length)]} {lastNames[_random.Next(lastNames.Length)]}";
        }

        private string GenerateEmail()
        {
            return $"user{_random.Next(1000, 9999)}@example.com";
        }

        private string GeneratePhoneNumber()
        {
            return $"+1-{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}";
        }

        private string GetRandomCategory()
        {
            var categories = new[] { "Electronics", "Books", "Clothing", "Home", "Sports" };
            return categories[_random.Next(categories.Length)];
        }

        private string GetRandomLoyaltyLevel()
        {
            var levels = new[] { "Bronze", "Silver", "Gold", "Platinum" };
            return levels[_random.Next(levels.Length)];
        }

        private string GetRandomCity()
        {
            var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" };
            return cities[_random.Next(cities.Length)];
        }

        private string GetRandomState()
        {
            var states = new[] { "NY", "CA", "IL", "TX", "AZ" };
            return states[_random.Next(states.Length)];
        }

        private string GenerateStreetAddress()
        {
            return $"{_random.Next(100, 9999)} {GetRandomStreetName()} {GetRandomStreetType()}";
        }

        private string GetRandomStreetName()
        {
            var names = new[] { "Main", "Oak", "Pine", "Maple", "Cedar", "Elm" };
            return names[_random.Next(names.Length)];
        }

        private string GetRandomStreetType()
        {
            var types = new[] { "St", "Ave", "Dr", "Ln", "Ct" };
            return types[_random.Next(types.Length)];
        }

        private string GenerateZipCode()
        {
            return _random.Next(10000, 99999).ToString();
        }

        private string GenerateDeliveryInstructions()
        {
            var instructions = new[]
            {
                "Leave at front door",
                "Ring doorbell",
                "Call upon arrival",
                "Leave with neighbor",
                "Signature required"
            };
            return instructions[_random.Next(instructions.Length)];
        }
    }
}