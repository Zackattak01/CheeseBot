using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.EfCore;
using CheeseBot.Entities;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class GuildSettingsService : DiscordClientService
    {
        private readonly Dictionary<ulong, GuildSettings> _guildSettingsCache;

        private readonly IServiceProvider _services;
        
        public GuildSettings DefaultSettings {get;}  
        
        public GuildSettingsService(IServiceProvider services, ILogger<GuildSettingsService> logger, DiscordClientBase client) : base(logger, client)
        {
            _services = services;
            _guildSettingsCache = new Dictionary<ulong, GuildSettings>();
            DefaultSettings = CreateDefaultGuildSettings();
        }

        private async Task<GuildSettings> GetGuildSettingsAsync(Snowflake guildId)
        {
            if (_guildSettingsCache.TryGetValue(guildId, out var settings))
                return settings;
            else
            {
                Logger.LogInformation($"Encountered guild: {guildId} for the first time.  Caching settings.");
                
                using var scope = _services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();
                var guildSettings = await dbContext.GuildSettings.FindAsync(guildId);
                
                if(!_guildSettingsCache.TryAdd(guildId, guildSettings))
                    Logger.LogWarning($"Could not cache settings for {guildId}!  Settings already cached?");

                return guildSettings;
            }
            
        }

        private GuildSettings CreateDefaultGuildSettings()
        {
            var prefixes = new HashSet<IPrefix>
            {
                new StringPrefix("?"), new MentionPrefix(Client.CurrentUser.Id)
            };

            return new GuildSettings();
        }

        public async Task<HashSet<IPrefix>> GetGuildPrefixesAsync(Snowflake guildId)
        {
            var settings = await GetGuildSettingsAsync(guildId);
            return settings.Prefixes;
        }
    }
}