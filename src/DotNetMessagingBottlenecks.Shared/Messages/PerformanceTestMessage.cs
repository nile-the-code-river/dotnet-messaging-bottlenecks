using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Messages
{
    internal class PerformanceTestMessage : IMessage
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        // 성능 테스트용 필드들
        public int SequenceNumber { get; init; }
        public string TestScenario { get; init; } = string.Empty;
        public int MessageSizeKB { get; init; }
        public byte[] Payload { get; init; } = Array.Empty<byte>();

        // 처리 시간 측정용
        public DateTime ProducedAt { get; init; } = DateTime.UtcNow;

        // 메타데이터
        public Dictionary<string, object> Metadata { get; init; } = new();
    }
}
