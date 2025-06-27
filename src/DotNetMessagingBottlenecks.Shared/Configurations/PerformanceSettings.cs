using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Configurations
{
    internal class PerformanceSettings
    {
        public const string SectionName = "Performance";

        // 메트릭 수집 설정
        public bool EnableMetrics { get; set; } = true;
        public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromSeconds(5);
        public bool EnableGCMetrics { get; set; } = true;
        public bool EnableMemoryMetrics { get; set; } = true;

        // 부하 테스트 설정
        public int DefaultThroughput { get; set; } = 100;
        public int MaxThroughput { get; set; } = 10000;
        public TimeSpan DefaultTestDuration { get; set; } = TimeSpan.FromMinutes(5);

        // 메시지 크기 설정 (KB)
        public int[] MessageSizes { get; set; } = { 1, 10, 100, 1000 };

        // 결과 저장 설정
        public string ResultsPath { get; set; } = "./results";
        public bool SaveRawData { get; set; } = true;
        public bool SaveAggregatedData { get; set; } = true;
    }
}
