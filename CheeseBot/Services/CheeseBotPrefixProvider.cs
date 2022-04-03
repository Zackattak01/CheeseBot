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
                return await _guildSettings.GetGuildPrefixesAsync(guildId);
            
            //DMs will use defaults
            return _defaultGuildSettingsProvider.DefaultPrefixes;
        }
        
    }
}