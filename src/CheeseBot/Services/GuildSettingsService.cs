using System.ComponentModel;

namespace CheeseBot.Services
{
    public class GuildSettingsService : CheeseBotService
    {
        private readonly Dictionary<Snowflake, GuildSettings> _guildSettingsCache;

        private readonly IServiceProvider _services;

        private readonly DefaultGuildSettingsProvider _defaultGuildSettingsProvider;

        public GuildSettingsService(
            IServiceProvider services, 
            DefaultGuildSettingsProvider defaultGuildSettingsProvider, 
            ILogger<GuildSettingsService> logger) 
            : base(logger)
        {
            _services = services;
            _defaultGuildSettingsProvider = defaultGuildSettingsProvider;
            _guildSettingsCache = new Dictionary<Snowflake, GuildSettings>();
        }

        private async Task<GuildSettings> FindOrCreateGuildSettings(Snowflake guildId)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();
            var guildSettings = await dbContext.GuildSettings.FindAsync(guildId);

            if (guildSettings is null)
            {
                guildSettings = _defaultGuildSettingsProvider.CreateDefaultGuildSettings(guildId);
                await dbContext.AddAsync(guildSettings);
                await dbContext.SaveChangesAsync();
            }
            
            return guildSettings;
        }

        public async Task<GuildSettings> GetGuildSettingsAsync(Snowflake guildId)
        {
            if (_guildSettingsCache.TryGetValue(guildId, out var settings))
                return settings;
            else
            {
                Logger.LogInformation("Encountered guild: {0} for the first time.  Caching settings.", guildId);

                var guildSettings = await FindOrCreateGuildSettings(guildId);
                
                if(!_guildSettingsCache.TryAdd(guildId, guildSettings))
                    Logger.LogWarning("Could not cache settings for {0}!  Settings already cached?", guildId);

                return guildSettings;
            }
            
        }

        //this method is really just a crutch for the extension method impls;  we dont need to see it normally
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task UpdateGuildSettingsAsync(GuildSettings settings)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();

            dbContext.Update(settings);
            await dbContext.SaveChangesAsync();
        }
    }
}