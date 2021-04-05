using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql.Replication.PgOutput.Messages;

namespace CheeseBot.Services
{
    public class CheeseBotPrefixProvider : IPrefixProvider
    {
        private readonly GuildSettingsService _guildSettings;
        
        private readonly DefaultGuildSettingsProvider _defaultGuildSettingsProvider;
        public CheeseBotPrefixProvider(GuildSettingsService guildSettings, DefaultGuildSettingsProvider defaultGuildSettingsProvider)
        {
            _guildSettings = guildSettings;
            _defaultGuildSettingsProvider = defaultGuildSettingsProvider;
        }

        public async ValueTask<IEnumerable<IPrefix>> GetPrefixesAsync(IGatewayUserMessage message)
        {
            
            if (message.GuildId is { } guildId)
            {
                var prefixes = await _guildSettings.GetGuildPrefixesAsync(guildId);
                return prefixes;
            }
        
            //DMs will use defaults
            return _defaultGuildSettingsProvider.DefaultPrefixes;
        }
        
    }
}