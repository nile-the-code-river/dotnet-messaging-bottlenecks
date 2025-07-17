using Core.Models;
using MassTransit;
using System.Diagnostics;
using System.Text.Json;
using Baseline.Producer;
using Microsoft.Extensions.Logging;

namespace Baseline.Consumer
{
    public class OrderConsumer : IConsumer<OrderMessage>
    {
        private readonly ILogger<OrderConsumer> _logger;
        private readonly Random _random = new();

        public OrderConsumer(ILogger<OrderConsumer> logger)
        {
            _logger = logger;

            try
            {
                _logger.LogWarning("=== Starting to load JSON data ===");

                string smMsg = SampleMessage.LoadJsonData(MessageSize.Small);
                _logger.LogWarning($"Small message loaded - Length: {smMsg.Length}");
                _logger.LogWarning($"Small message content: {smMsg.Substring(0, Math.Min(200, smMsg.Length))}...");

                string mdMsg = SampleMessage.LoadJsonData(MessageSize.Medium);
                _logger.LogWarning($"Medium message loaded - Length: {mdMsg.Length}");

                string lgMsg = SampleMessage.LoadJsonData(MessageSize.Large);
                _logger.LogWarning($"Large message loaded - Length: {lgMsg.Length}");

                _logger.LogWarning("=== JSON data loading completed ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load JSON data");
                throw;
            }

        }

        public async Task Consume(ConsumeContext<OrderMessage> context)
        {
            var message = context.Message;
            var startTime = DateTime.UtcNow;
            var processingStopwatch = Stopwatch.StartNew(); // ⭐ 추가

            _logger.LogInformation("[Consumer] Processing order {OrderId} with {ItemCount} items",
                message.OrderId, message.Items.Count);

            try
            {
                var processedOrder = await ProcessOrder(message);

                var result = new OrderProcessingResult
                {
                    OrderId = message.OrderId,
                    ProcessedAt = DateTime.UtcNow,
                    ProcessingDuration = DateTime.UtcNow - startTime,
                    Status = "Completed",
                    ProcessedItems = processedOrder.ProcessedItems,
                    TotalProcessingCost = CalculateProcessingCost(processedOrder),
                    ValidationResults = ValidateOrder(message),
                    ProcessingNotes = GenerateProcessingNotes(processedOrder)
                };

                LogOrderProcessing(message, result);
                await NotifyRelatedSystems(result);

                // 처리 시간 시뮬레이션
                await Task.Delay(_random.Next(5, 20));

                processingStopwatch.Stop(); // ⭐ 추가


                // ✅ 이걸로 교체
                if (OrderProducer.IsTrackerActive())
                {
                    OrderProducer.NotifyMessageProcessed(processingStopwatch.Elapsed.TotalMilliseconds);
                    Console.WriteLine($"[DEBUG] Notified producer: {processingStopwatch.Elapsed.TotalMilliseconds:F2}ms");
                }
                else
                {
                    Console.WriteLine("[DEBUG] Tracker not active - notification skipped");
                }

                _logger.LogInformation("[Consumer] Completed order {OrderId} in {Duration}ms",
                    message.OrderId, result.ProcessingDuration.TotalMilliseconds);

                _logger.LogInformation("[Consumer] Completed order {OrderId} in {Duration}ms",
                    message.OrderId, result.ProcessingDuration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                processingStopwatch.Stop(); // ⭐ 추가
                _logger.LogError(ex, "[Consumer] Failed to process order {OrderId}", message.OrderId);
                throw;
            }
        }

        private async Task<ProcessedOrder> ProcessOrder(OrderMessage orderMessage)
        {
            var processedOrder = new ProcessedOrder // 새 객체 생성
            {
                OriginalOrderId = orderMessage.OrderId,
                ProcessedItems = new List<ProcessedOrderItem>(), // 새 리스트
                CustomerData = ProcessCustomerData(orderMessage.CustomerInfo), // 새 객체
                ShippingData = ProcessShippingData(orderMessage.ShippingAddress), // 새 객체
                ProcessingMetadata = CreateProcessingMetadata() // 새 딕셔너리
            };

            // 각 아이템 처리 (LINQ 대신 foreach - 하지만 여전히 할당 발생)
            foreach (var item in orderMessage.Items)
            {
                var processedItem = await ProcessOrderItem(item); // 새 객체 생성
                processedOrder.ProcessedItems.Add(processedItem);
            }

            // 주문 요약 생성
            processedOrder.OrderSummary = processedOrder.ToString();

            return processedOrder;
        }


        private async Task<ProcessedOrderItem> ProcessOrderItem(OrderItemDto item)
        {
            // 실제 아이템 처리 시뮬레이션
            await Task.Delay(1);

            return new ProcessedOrderItem // 새 객체
            {
                OriginalProductId = item.ProductId,
                ProcessedProductName = item.ProductName?.ToUpper(), // 새 문자열
                ValidatedQuantity = item.Quantity,
                FinalPrice = item.Price * (decimal)0.95, // 할인 적용
                ProcessingTimestamp = DateTime.UtcNow,
                ItemNotes = GenerateItemProcessingNotes(item), // 문자열 생성
                CategoryInfo = ProcessCategoryInfo(item.Category) // 새 객체
            };
        }

        private ProcessedCustomerData ProcessCustomerData(CustomerInfo customerInfo)
        {
            return new ProcessedCustomerData // 새 객체
            {
                CustomerId = Guid.NewGuid(),
                ValidatedName = ValidateCustomerName(customerInfo.Name), // 문자열 처리
                ValidatedEmail = ValidateEmail(customerInfo.Email), // 문자열 처리
                ValidatedPhone = ValidatePhone(customerInfo.Phone), // 문자열 처리
                LoyaltyInfo = ProcessLoyaltyInfo(customerInfo.LoyaltyLevel), // 새 객체
                ProcessingFlags = GenerateCustomerFlags(customerInfo) // 새 리스트
            };
        }

        private ProcessedShippingData ProcessShippingData(ShippingAddress address)
        {
            return new ProcessedShippingData // 새 객체
            {
                ValidatedAddress = ValidateAddress(address), // 새 객체
                EstimatedDeliveryDate = CalculateDeliveryDate(address),
                ShippingCost = CalculateShippingCost(address),
                DeliveryOptions = GetAvailableDeliveryOptions(address), // 새 리스트
                ShippingNotes = GenerateShippingNotes(address) // 문자열 생성
            };
        }

        private Dictionary<string, object> CreateProcessingMetadata()
        {
            return new Dictionary<string, object> // 새 딕셔너리
            {
                ["processedBy"] = Environment.MachineName,
                ["processedAt"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                ["processingVersion"] = "1.0.0",
                ["threadId"] = Thread.CurrentThread.ManagedThreadId,
                ["memoryUsage"] = GC.GetTotalMemory(false),
                ["randomId"] = Guid.NewGuid().ToString()
            };
        }

        private List<string> ValidateOrder(OrderMessage order)
        {
            var validationResults = new List<string>(); // 새 리스트

            // 일반적인 검증 로직
            if (order.Items == null || !order.Items.Any())
                validationResults.Add("Order must contain at least one item");

            if (order.TotalAmount <= 0)
                validationResults.Add("Order total must be greater than zero");

            if (string.IsNullOrWhiteSpace(order.CustomerId))
                validationResults.Add("Customer ID is required");

            // 각 아이템에 대한 검증
            foreach (var item in order.Items ?? new List<OrderItemDto>())
            {
                if (item.Quantity <= 0)
                    validationResults.Add($"Invalid quantity for product {item.ProductId}");

                if (item.Price <= 0)
                    validationResults.Add($"Invalid price for product {item.ProductId}");
            }

            return validationResults;
        }

        private string GenerateProcessingNotes(ProcessedOrder order)
        {
            // 문자열 연결을 통한 노트 생성
            var notes = $"Order {order.OriginalOrderId} processed successfully. ";
            notes += $"Total items: {order.ProcessedItems.Count}. ";
            notes += $"Processing completed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}. ";

            if (order.ProcessedItems.Any(x => x.FinalPrice != x.ValidatedQuantity * 10))
                notes += "Discount applied to some items. ";

            notes += $"Processing metadata: {JsonSerializer.Serialize(order.ProcessingMetadata)}";

            return notes;
        }

        private decimal CalculateProcessingCost(ProcessedOrder order)
        {
            // 처리 비용 계산 (새로운 계산 객체들 생성)
            var baseCost = order.ProcessedItems.Sum(x => 0.10m);
            var complexityCost = order.ProcessedItems.Count * 0.05m;
            var priorityCost = DateTime.UtcNow.Hour > 17 ? 0.25m : 0.0m;

            return baseCost + complexityCost + priorityCost;
        }

        private void LogOrderProcessing(OrderMessage message, OrderProcessingResult result)
        {
            // 상세한 로깅 (문자열 생성 많음)
            var logData = new
            {
                OrderId = message.OrderId,
                CustomerId = message.CustomerId,
                ItemCount = message.Items?.Count ?? 0,
                TotalAmount = message.TotalAmount,
                ProcessingDuration = result.ProcessingDuration.TotalMilliseconds,
                Status = result.Status,
                ValidationIssues = result.ValidationResults?.Count ?? 0,
                ProcessingTimestamp = result.ProcessedAt.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };

            var logMessage = JsonSerializer.Serialize(logData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            _logger.LogInformation("Order processing completed: {LogData}", logMessage);
        }

        private async Task NotifyRelatedSystems(OrderProcessingResult result)
        {
            // 관련 시스템 알림 (추가 객체 생성)
            var notifications = new List<SystemNotification>(); // 새 리스트

            notifications.Add(new SystemNotification
            {
                System = "Inventory",
                Message = $"Order {result.OrderId} processed",
                Timestamp = DateTime.UtcNow,
                Data = CreateNotificationData(result, "inventory")
            });

            notifications.Add(new SystemNotification
            {
                System = "Shipping",
                Message = $"Order {result.OrderId} ready for shipping",
                Timestamp = DateTime.UtcNow,
                Data = CreateNotificationData(result, "shipping")
            });

            // 알림 전송 시뮬레이션
            foreach (var notification in notifications)
            {
                await Task.Delay(1); // 네트워크 호출 시뮬레이션
                _logger.LogInformation("Notified {System}: {Message}",
                    notification.System, notification.Message);
            }
        }

        // 헬퍼 메서드들 (모두 새 객체/문자열 생성)
        private string ValidateCustomerName(string name) => name?.Trim() ?? "Unknown";
        private string ValidateEmail(string email) => email?.ToLower().Trim() ?? "";
        private string ValidatePhone(string phone) => phone?.Replace("-", "").Replace(" ", "") ?? "";

        private LoyaltyInfo ProcessLoyaltyInfo(string level) => new() { Level = level, Points = _random.Next(100, 1000) };
        private List<string> GenerateCustomerFlags(CustomerInfo info) => new() { "Validated", "Active" };

        private ValidatedAddress ValidateAddress(ShippingAddress addr) => new()
        {
            Street = addr.Street,
            City = addr.City,
            State = addr.State,
            IsValid = true
        };

        private DateTime CalculateDeliveryDate(ShippingAddress addr) => DateTime.UtcNow.AddDays(_random.Next(1, 7));
        private decimal CalculateShippingCost(ShippingAddress addr) => (decimal)(_random.NextDouble() * 20 + 5);

        private List<string> GetAvailableDeliveryOptions(ShippingAddress addr) =>
            new() { "Standard", "Express", "Overnight" };

        private string GenerateShippingNotes(ShippingAddress addr) =>
            $"Shipping to {addr.City}, {addr.State}. Estimated delivery in 3-5 business days.";

        private string GenerateItemProcessingNotes(OrderItemDto item) =>
            $"Item {item.ProductId} processed successfully. Category: {item.Category}";

        private CategoryInfo ProcessCategoryInfo(string category) =>
            new() { Name = category, TaxRate = 0.08m, IsRestricted = false };

        private Dictionary<string, object> CreateNotificationData(OrderProcessingResult result, string system) =>
            new()
            {
                ["orderId"] = result.OrderId,
                ["system"] = system,
                ["timestamp"] = DateTime.UtcNow.ToString()
            };
    }
}