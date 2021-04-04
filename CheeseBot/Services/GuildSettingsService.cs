using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using Disqord;
using Disqord.Bot;
using Disqord.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class GuildSettingsService : DiscordClientService
    {
        private readonly Dictionary<ulong, GuildSettings> _guildSettingsCache;

        private readonly IServiceProvider _services;

        private readonly DefaultGuildSettingsProvider _defaultGuildSettingsProvider;
        
        
        public GuildSettingsService(IServiceProvider services, 
                                    DefaultGuildSettingsProvider defaultGuildSettingsProvider, 
                                    ILogger<GuildSettingsService> logger, 
                                    DiscordClientBase client) 
                                    : base(logger, client)
        {
            _services = services;
            _defaultGuildSettingsProvider = defaultGuildSettingsProvider;
            _guildSettingsCache = new Dictionary<ulong, GuildSettings>();
        }

        public async Task<GuildSettings> GetGuildSettingsAsync(Snowflake guildId)
        {
            if (_guildSettingsCache.TryGetValue(guildId, out var settings))
                return settings;
            else
            {
                Logger.LogInformation($"Encountered guild: {guildId} for the first time.  Caching settings.");

                var guildSettings = await FindOrCreateGuildSettings(guildId);
                
                if(!_guildSettingsCache.TryAdd(guildId, guildSettings))
                    Logger.LogWarning($"Could not cache settings for {guildId}!  Settings already cached?");

                return guildSettings;
            }
            
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

        public async Task<HashSet<IPrefix>> GetGuildPrefixesAsync(Snowflake guildId)
        {
            var settings = await GetGuildSettingsAsync(guildId);
            return settings.Prefixes;
        }
    }
}