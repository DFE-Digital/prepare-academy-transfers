using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.Extensions.Logging;

namespace Frontend.BackgroundServices
{
    public class PerformanceDataChannel
    {
        private const int MaxMessagesInChannel = 100;

        private readonly Channel<string> _channel;
        private readonly ILogger<PerformanceDataChannel> _logger;

        public PerformanceDataChannel(ILogger<PerformanceDataChannel> logger)
        {
            var options = new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleWriter = false,
                SingleReader = true                
            };

            _channel = Channel.CreateBounded<string>(options);

            _logger = logger;
        }
        
        public async Task<bool> AddAcademyAsync(string academyUkprn, CancellationToken ct = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(academyUkprn))
                {
                    Log.ChannelMessageWritten(_logger, academyUkprn);

                    return true;
                }
            }

            return false;
        }
        
        public IAsyncEnumerable<string> ReadAllAsync(CancellationToken ct = default) =>
            _channel.Reader.ReadAllAsync(ct);

        public bool TryCompleteWriter(Exception ex = null) => _channel.Writer.TryComplete(ex);
        internal static class EventIds
        {
            public static readonly EventId ChannelMessageWritten = new EventId(100, "ChannelMessageWritten");
        }
        private static class Log
        {
            private static readonly Action<ILogger, string, Exception> _channelMessageWritten = LoggerMessage.Define<string>(
                LogLevel.Information,
                EventIds.ChannelMessageWritten,
                "Academy Ukprn {ukprn} was written to the channel.");

            public static void ChannelMessageWritten(ILogger logger, string academyUkprn)
            {
                _channelMessageWritten(logger, academyUkprn, null);
            }
        }
    }
}