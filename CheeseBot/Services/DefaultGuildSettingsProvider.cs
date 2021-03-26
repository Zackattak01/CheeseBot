using System.Collections.Generic;
using CheeseBot.Entities;
using Disqord;
using Disqord.Bot;
using Disqord.Hosting;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class DefaultGuildSettingsProvider : DiscordClientService
    {
        public HashSet<IPrefix> DefaultPrefixes { get; } 
        
        public DefaultGuildSettingsProvider(ILogger<DefaultGuildSettingsProvider> logger, DiscordClientBase client) 
            : base(logger, client)
        {
            DefaultPrefixes = new HashSet<IPrefix>
            {
                new StringPrefix("?"),
                new MentionPrefix(client.CurrentUser.Id)
            };
            
            logger.LogInformation("Default settings setup!");
        }

        public GuildSettings CreateDefaultGuildSettings(ulong guildId)
        {
            return new GuildSettings(guildId, DefaultPrefixes);
        }
    }
}