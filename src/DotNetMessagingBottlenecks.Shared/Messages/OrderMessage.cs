using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Messages
{
    internal class OrderMessage : IMessage
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        // 비즈니스 데이터
        public int OrderId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public List<OrderItem> Items { get; init; } = new();
        public decimal TotalAmount { get; init; }
        public OrderProcessingType ProcessingType { get; init; }
    }

    public record OrderItem(
        string ProductName,
        int Quantity,
        decimal Price
    );

    public enum OrderProcessingType
    {
        Simple,        // CPU 최소 사용
        CpuIntensive,  // CPU 집약적 (암호화, 압축 등)
        MemoryIntensive, // 메모리 집약적 (대용량 객체 생성)
        IoIntensive    // I/O 집약적 (파일, DB 접근)
    }
}
