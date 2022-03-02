using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Frontend.BackgroundServices
{
    public class PerformanceDataProcessingService : BackgroundService
    {
        private readonly ILogger<PerformanceDataProcessingService> _logger;
        private readonly PerformanceDataChannel _performanceDataChannel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCache _distributedCache;

        public PerformanceDataProcessingService(ILogger<PerformanceDataProcessingService> logger,
            PerformanceDataChannel performanceDataChannel, IServiceProvider serviceProvider, IDistributedCache distributedCache)
        {
            _logger = logger;
            _performanceDataChannel = performanceDataChannel;
            _serviceProvider = serviceProvider;
            _distributedCache = distributedCache;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var academyUrn in _performanceDataChannel.ReadAllAsync())
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IEducationPerformance>();
                await processor.GetByAcademyUrn(academyUrn);
            }
        }
        
        internal static class EventIds
        {
            public static readonly EventId StartedProcessing = new EventId(100, "StartedProcessing");
            public static readonly EventId ProcessorStopping = new EventId(101, "ProcessorStopping");
            public static readonly EventId StoppedProcessing = new EventId(102, "StoppedProcessing");
            public static readonly EventId ProcessedMessage = new EventId(110, "ProcessedMessage");
        }

        private static class Log
        {
            private static readonly Action<ILogger, string, Exception> _processedMessage = LoggerMessage.Define<string>(
                LogLevel.Debug,
                EventIds.ProcessedMessage,
                "Read and processed message with ID '{MessageId}' from the channel.");

            public static void StartedProcessing(ILogger logger) => logger.Log(LogLevel.Trace, EventIds.StartedProcessing, "Started message processing service.");

            public static void ProcessorStopping(ILogger logger) => logger.Log(LogLevel.Information, EventIds.ProcessorStopping, "Message processing stopping due to app termination!");

            public static void StoppedProcessing(ILogger logger) => logger.Log(LogLevel.Trace, EventIds.StoppedProcessing, "Stopped message processing service.");

            public static void ProcessedMessage(ILogger logger, string messageId) => _processedMessage(logger, messageId, null);
        }
    }
}