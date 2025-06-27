using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Configurations
{
    internal class LoggingSettings
    {
        public const string SectionName = "Logging";

        public string MinimumLevel { get; set; } = "Information";
        public bool EnableConsoleLogging { get; set; } = true;
        public bool EnableFileLogging { get; set; } = true;
        public string LogFilePath { get; set; } = "./logs/app-.log";
        public int RetainedFileCountLimit { get; set; } = 31;

        // 통합: CreateLogger 메서드를 여기에 포함
        public ILogger CreateLogger()
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Is(ParseLogLevel(MinimumLevel))
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "DotNetMessagingBottlenecks");

            if (EnableConsoleLogging)
            {
                loggerConfig.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
            }

            if (EnableFileLogging)
            {
                loggerConfig.WriteTo.File(
                    path: LogFilePath,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
            }

            return loggerConfig.CreateLogger();
        }

        private LogEventLevel ParseLogLevel(string level)
        {
            return level.ToUpperInvariant() switch
            {
                "VERBOSE" => LogEventLevel.Verbose,
                "DEBUG" => LogEventLevel.Debug,
                "INFORMATION" => LogEventLevel.Information,
                "WARNING" => LogEventLevel.Warning,
                "ERROR" => LogEventLevel.Error,
                "FATAL" => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }
    }
}
