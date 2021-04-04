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

namespace CheeseBot.Services
{
    public class CheeseBotPrefixProvider : /*DiscordClientService,*/ IPrefixProvider
    {
        // private readonly GuildSettingsService _guildSettings;
        //
        // private readonly DefaultGuildSettingsProvider _defaultGuildSettingsProvider;
        // public CheeseBotPrefixProvider(GuildSettingsService guildSettings, DefaultGuildSettingsProvider defaultGuildSettingsProvider)
        // {
        //     _guildSettings = guildSettings;
        //     _defaultGuildSettingsProvider = defaultGuildSettingsProvider;
        // }

        // public CheeseBotPrefixProvider(
        //     GuildSettingsService guildSettings,
        //     DefaultGuildSettingsProvider defaultGuildSettingsProvider,
        //     ILogger<CheeseBotPrefixProvider> logger,
        //     DiscordClientBase client)
        //     : base(logger, client)
        // {
        //     logger.LogInformation("Prefix service");
        //     _guildSettings = guildSettings;
        //     _defaultGuildSettingsProvider = defaultGuildSettingsProvider;
        // }

        // public async ValueTask<IEnumerable<IPrefix>> GetPrefixesAsync(IGatewayUserMessage message)
        // {
        //     
        //     if (message.GuildId is { } guildId)
        //     {
        //         var prefixes = await _guildSettings.GetGuildPrefixesAsync(guildId);
        //         return prefixes;
        //     }
        //
        //     //DMs will use defaults
        //     //return _defaultGuildSettingsProvider.DefaultPrefixes;
        //     throw new Exception();
        // }
        public ValueTask<IEnumerable<IPrefix>> GetPrefixesAsync(IGatewayUserMessage message)
        {

            return new(new[] {new StringPrefix("?")});
        }
    }
}