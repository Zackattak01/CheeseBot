using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class GuildSettingsService : CheeseBotService
    {
        private readonly Dictionary<ulong, GuildSettings> _guildSettingsCache;

        private readonly IServiceProvider _services;

        private readonly DefaultGuildSettingsProvider _defaultGuildSettingsProvider;


        public GuildSettingsService(IServiceProvider services, 
                                    DefaultGuildSettingsProvider defaultGuildSettingsProvider, 
                                    ILogger<GuildSettingsService> logger) 
                                    : base(logger)
        {
            _services = services;
            _defaultGuildSettingsProvider = defaultGuildSettingsProvider;
            _guildSettingsCache = new Dictionary<ulong, GuildSettings>();
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
                Logger.LogInformation($"Encountered guild: {guildId} for the first time.  Caching settings.");

                var guildSettings = await FindOrCreateGuildSettings(guildId);
                
                if(!_guildSettingsCache.TryAdd(guildId, guildSettings))
                    Logger.LogWarning($"Could not cache settings for {guildId}!  Settings already cached?");

                return guildSettings;
            }
            
        }

        public async Task<HashSet<IPrefix>> GetGuildPrefixesAsync(Snowflake guildId)
        {
            var settings = await GetGuildSettingsAsync(guildId);
            return settings.Prefixes;
        }

        public async Task AddPrefixAsync(Snowflake guildId, IPrefix prefix)
        {
            var guildSettings = await GetGuildSettingsAsync(guildId);
            
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();
            
            //TODO: This is dangerous; prefixes should not be directly mutable
            guildSettings.Prefixes.Add(prefix);

            dbContext.Update(guildSettings);
            await dbContext.SaveChangesAsync();
        }
        
        public async Task RemovePrefixAsync(Snowflake guildId, IPrefix prefix)
        {
            var guildSettings = await GetGuildSettingsAsync(guildId);
            
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();
            
            //TODO: This is dangerous; prefixes should not be directly mutable
            guildSettings.Prefixes.Remove(prefix);

            dbContext.Update(guildSettings);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> GuildIsPermittedAsync(Snowflake guildId)
        {
            var settings = await GetGuildSettingsAsync(guildId);
            return settings.IsPermitted;
        }

        public async Task PermitGuildAsync(Snowflake guildId)
        {
            var guildSettings = await GetGuildSettingsAsync(guildId);
            
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();

            guildSettings.IsPermitted = true;

            dbContext.Update(guildSettings);
            await dbContext.SaveChangesAsync();
        }
    }
}