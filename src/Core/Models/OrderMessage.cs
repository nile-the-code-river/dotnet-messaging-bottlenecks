namespace Core.Models
{
    public class OrderMessage
    {
        public Guid OrderId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public CustomerInfo CustomerInfo { get; set; } = new();
        public ShippingAddress ShippingAddress { get; set; } = new();
        public string OrderNotes { get; set; } = string.Empty;
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CustomerInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LoyaltyLevel { get; set; } = string.Empty;
        public string PreferredLanguage { get; set; } = string.Empty;
        public DateTime AccountCreatedDate { get; set; }
    }

    public class ShippingAddress
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string DeliveryInstructions { get; set; } = string.Empty;
    }

    // Consumer에서 사용할 결과 클래스들
    public class OrderProcessingResult
    {
        public Guid OrderId { get; set; }
        public DateTime ProcessedAt { get; set; }
        public TimeSpan ProcessingDuration { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ProcessedOrderItem> ProcessedItems { get; set; } = new();
        public decimal TotalProcessingCost { get; set; }
        public List<string> ValidationResults { get; set; } = new();
        public string ProcessingNotes { get; set; } = string.Empty;
    }

    public class ProcessedOrder
    {
        public Guid OriginalOrderId { get; set; }
        public List<ProcessedOrderItem> ProcessedItems { get; set; } = new();
        public ProcessedCustomerData CustomerData { get; set; } = new();
        public ProcessedShippingData ShippingData { get; set; } = new();
        public Dictionary<string, object> ProcessingMetadata { get; set; } = new();
        public string OrderSummary { get; set; } = string.Empty;
    }

    public class ProcessedOrderItem
    {
        public string OriginalProductId { get; set; } = string.Empty;
        public string ProcessedProductName { get; set; } = string.Empty;
        public int ValidatedQuantity { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime ProcessingTimestamp { get; set; }
        public string ItemNotes { get; set; } = string.Empty;
        public CategoryInfo CategoryInfo { get; set; } = new();
    }

    public class ProcessedCustomerData
    {
        public Guid CustomerId { get; set; }
        public string ValidatedName { get; set; } = string.Empty;
        public string ValidatedEmail { get; set; } = string.Empty;
        public string ValidatedPhone { get; set; } = string.Empty;
        public LoyaltyInfo LoyaltyInfo { get; set; } = new();
        public List<string> ProcessingFlags { get; set; } = new();
    }

    public class ProcessedShippingData
    {
        public ValidatedAddress ValidatedAddress { get; set; } = new();
        public DateTime EstimatedDeliveryDate { get; set; }
        public decimal ShippingCost { get; set; }
        public List<string> DeliveryOptions { get; set; } = new();
        public string ShippingNotes { get; set; } = string.Empty;
    }

    public class LoyaltyInfo
    {
        public string Level { get; set; } = string.Empty;
        public int Points { get; set; }
    }

    public class ValidatedAddress
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public bool IsValid { get; set; }
    }

    public class CategoryInfo
    {
        public string Name { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public bool IsRestricted { get; set; }
    }

    public class SystemNotification
    {
        public string System { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }
}