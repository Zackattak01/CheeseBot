using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Utilities;
using Microsoft.Extensions.Options;

namespace CheeseBot.Services
{
    public class CheeseBotPrefixProvider : IPrefixProvider
    {
        private readonly GuildSettingsService _guildSettings;
        
        
        public CheeseBotPrefixProvider(DiscordBot bot, GuildSettingsService guildSettings)
        {
            _guildSettings = guildSettings;

        }
        
        public async ValueTask<IEnumerable<IPrefix>> GetPrefixesAsync(IGatewayUserMessage message)
        {
            if (message.GuildId is { } guildId)
            {
                var prefixes = await _guildSettings.GetGuildPrefixesAsync(guildId);
                return prefixes;
            }

            //DMs will use defaults
            return _guildSettings.DefaultSettings.Prefixes;
        }
    }
}