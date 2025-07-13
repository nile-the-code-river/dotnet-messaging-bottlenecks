using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Monitoring
{
    public class PerformanceTracker
    {
        private readonly DateTime _startTime;
        private readonly int _initialGC0, _initialGC1, _initialGC2;
        private readonly long _initialMemory;
        private readonly Stopwatch _stopwatch;

        private int _messagesSent = 0;
        private int _messagesProcessed = 0;
        private readonly List<double> _processingTimes = new();

        public PerformanceTracker()
        {
            _startTime = DateTime.UtcNow;
            _initialGC0 = GC.CollectionCount(0);
            _initialGC1 = GC.CollectionCount(1);
            _initialGC2 = GC.CollectionCount(2);
            _initialMemory = GC.GetTotalMemory(false);
            _stopwatch = Stopwatch.StartNew();
        }

        public void MessageSent() => Interlocked.Increment(ref _messagesSent);

        public void MessageProcessed(double processingTimeMs)
        {
            Interlocked.Increment(ref _messagesProcessed);
            lock (_processingTimes)
            {
                _processingTimes.Add(processingTimeMs);
            }
        }

        public void LogCurrentStats()
        {
            var elapsed = _stopwatch.Elapsed;
            var currentMemory = GC.GetTotalMemory(false) / 1024 / 1024; // MB
            var throughputSent = _messagesSent / elapsed.TotalSeconds;
            var throughputProcessed = _messagesProcessed / elapsed.TotalSeconds;

            var gc0 = GC.CollectionCount(0) - _initialGC0;
            var gc1 = GC.CollectionCount(1) - _initialGC1;
            var gc2 = GC.CollectionCount(2) - _initialGC2;

            Console.WriteLine($"""
            ⏱️  CURRENT STATS (Runtime: {elapsed.TotalSeconds:F1}s)
            📤 Sent: {_messagesSent:N0} ({throughputSent:F1}/sec)
            📥 Processed: {_messagesProcessed:N0} ({throughputProcessed:F1}/sec)
            🧠 Memory: {currentMemory:N0}MB
            🗑️  GC: Gen0={gc0}, Gen1={gc1}, Gen2={gc2}
            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            """);
        }

        public void LogFinalReport()
        {
            var elapsed = _stopwatch.Elapsed;
            var memoryDelta = (GC.GetTotalMemory(false) - _initialMemory) / 1024 / 1024;

            var gc0 = GC.CollectionCount(0) - _initialGC0;
            var gc1 = GC.CollectionCount(1) - _initialGC1;
            var gc2 = GC.CollectionCount(2) - _initialGC2;

            double avgProcessingTime = 0, p95ProcessingTime = 0, p99ProcessingTime = 0;

            lock (_processingTimes)
            {
                if (_processingTimes.Any())
                {
                    _processingTimes.Sort();
                    avgProcessingTime = _processingTimes.Average();
                    p95ProcessingTime = _processingTimes[(int)(_processingTimes.Count * 0.95)];
                    p99ProcessingTime = _processingTimes[(int)(_processingTimes.Count * 0.99)];
                }
            }

            Console.WriteLine($"""
            
            ═══════════════ FINAL PERFORMANCE REPORT ═══════════════
            ⏱️  Total Duration: {elapsed.TotalSeconds:F1} seconds
            
            📊 THROUGHPUT:
              • Messages Sent: {_messagesSent:N0} ({_messagesSent / elapsed.TotalSeconds:F1}/sec)
              • Messages Processed: {_messagesProcessed:N0} ({_messagesProcessed / elapsed.TotalSeconds:F1}/sec)
              • Success Rate: {(_messagesProcessed * 100.0 / Math.Max(_messagesSent, 1)):F1}%
            
            ⚡ LATENCY:
              • Average: {avgProcessingTime:F2}ms
              • P95: {p95ProcessingTime:F2}ms  
              • P99: {p99ProcessingTime:F2}ms
            
            🧠 MEMORY:
              • Memory Delta: {memoryDelta:+0;-0}MB
              • Peak Memory: {GC.GetTotalMemory(false) / 1024 / 1024:N0}MB
            
            🗑️  GARBAGE COLLECTION:
              • Gen 0 Collections: {gc0:N0} ({gc0 / elapsed.TotalMinutes:F1}/min)
              • Gen 1 Collections: {gc1:N0} ({gc1 / elapsed.TotalMinutes:F1}/min)  
              • Gen 2 Collections: {gc2:N0} ({gc2 / elapsed.TotalMinutes:F1}/min)
              • Total GC Events: {gc0 + gc1 + gc2:N0}
            
            💡 GC PRESSURE ANALYSIS:
              • Allocation Rate: ~{((GC.GetTotalMemory(false) - _initialMemory) / elapsed.TotalSeconds / 1024 / 1024):F1}MB/sec
              • GC Overhead: ~{((gc0 * 1 + gc1 * 10 + gc2 * 100) / elapsed.TotalMilliseconds * 100):F2}%
            ════════════════════════════════════════════════════════
            """);
        }
    }
}
