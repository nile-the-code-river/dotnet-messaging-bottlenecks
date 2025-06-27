using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Messages
{
    internal class LoadTestControlMessage : IMessage
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        public LoadTestCommand Command { get; init; }
        public int TargetThroughput { get; init; } // TPS
        public TimeSpan Duration { get; init; }
        public LoadTestScenario Scenario { get; init; }
    }
    public enum LoadTestCommand
    {
        Start,
        Stop,
        Pause,
        Resume,
        ChangeLoad
    }

    public enum LoadTestScenario
    {
        GradualIncrease,    // 점진적 부하 증가
        MessageSizeTest,    // 메시지 크기별 테스트
        ConcurrencyTest,    // 동시성 테스트
        StabilityTest       // 장기간 안정성 테스트
    }
}
