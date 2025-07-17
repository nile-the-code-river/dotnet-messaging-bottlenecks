using Core.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baseline.Consumer
{
    internal class SampleConsumer : IConsumer<SampleMessage>
    {
        private readonly ILogger<SampleConsumer> _logger;

        public SampleConsumer(ILogger<SampleConsumer> logger)
        {
            _logger = logger;
        }

        private void LoadJsondata()
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

        public Task Consume(ConsumeContext<SampleMessage> context)
        {
            return Task.CompletedTask;
        }
    }
}
