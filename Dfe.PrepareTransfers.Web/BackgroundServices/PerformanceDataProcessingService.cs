using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dfe.PrepareTransfers.Web.BackgroundServices
{
    public class PerformanceDataProcessingService : BackgroundService
    {
        private readonly ILogger<PerformanceDataProcessingService> _logger;
        private readonly PerformanceDataChannel _performanceDataChannel;
        private readonly IServiceProvider _serviceProvider;

        public PerformanceDataProcessingService(ILogger<PerformanceDataProcessingService> logger,
            PerformanceDataChannel performanceDataChannel, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _performanceDataChannel = performanceDataChannel;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.StartedProcessing(_logger);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.StoppedProcessing(_logger);
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var academyUkprn in _performanceDataChannel.ReadAllAsync())
            {
                using var scope = _serviceProvider.CreateScope();
                var educationPerformance = scope.ServiceProvider.GetRequiredService<IEducationPerformance>();
                var academies = scope.ServiceProvider.GetRequiredService<IAcademies>();
                var academy = await academies.GetAcademyByUkprn(academyUkprn);
                await educationPerformance.GetByAcademyUrn(academy.Result.Urn);
                Log.ProcessedMessage(_logger, academyUkprn);
            }
        }

        internal static class EventIds
        {
            public static readonly EventId StartedProcessing = new EventId(100, "StartedProcessing");
            public static readonly EventId StoppedProcessing = new EventId(102, "StoppedProcessing");
            public static readonly EventId ProcessedMessage = new EventId(110, "ProcessedMessage");
        }

        private static class Log
        {
            private static readonly Action<ILogger, string, Exception> _processedMessage = LoggerMessage.Define<string>(
                LogLevel.Information,
                EventIds.ProcessedMessage,
                "Read and processed message with ID '{MessageId}' from the channel.");

            public static void StartedProcessing(ILogger logger) => logger.Log(LogLevel.Information,
                EventIds.StartedProcessing, "Started message processing service.");
            public static void StoppedProcessing(ILogger logger) => logger.Log(LogLevel.Information,
                EventIds.StoppedProcessing, "Stopped message processing service.");
            public static void ProcessedMessage(ILogger logger, string messageId) =>
                _processedMessage(logger, messageId, null);
        }
    }
}