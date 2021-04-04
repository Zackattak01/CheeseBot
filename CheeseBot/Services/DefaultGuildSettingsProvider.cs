using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CheeseBot.EfCore.Entities;
using Disqord;
using Disqord.Bot;
using Disqord.Hosting;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class DefaultGuildSettingsProvider : DiscordClientService
    {
        public HashSet<IPrefix> DefaultPrefixes { get; private set; } 
        
        public DefaultGuildSettingsProvider(ILogger<DefaultGuildSettingsProvider> logger, DiscordClientBase client) 
            : base(logger, client)
        {
            
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //TODO: Is it right to just pass along the CT? Never worked with CTs really
            //await Client.WaitUntilReadyAsync(cancellationToken);
            DefaultPrefixes = new HashSet<IPrefix>
            {
                new StringPrefix("?"),
                //new MentionPrefix(Client.CurrentUser.Id)
            };
            
            Logger.LogInformation("Default settings setup!");
        }

        public GuildSettings CreateDefaultGuildSettings(Snowflake guildId)
        {
            return new(guildId, DefaultPrefixes);
        }
    }
}