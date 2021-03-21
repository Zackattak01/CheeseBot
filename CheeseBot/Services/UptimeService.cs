using System;
using Disqord;
using Disqord.Hosting;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public sealed class UptimeService : DiscordClientService
    {
        public TimeSpan Uptime => DateTime.Now - StartTime;

        public DateTime StartTime { get; }

        public UptimeService(ILogger<UptimeService> logger, DiscordClientBase client) : base(logger, client)
        {
            StartTime = DateTime.Now;
        }
    }
}