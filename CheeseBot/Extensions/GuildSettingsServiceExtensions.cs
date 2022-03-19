using System.Collections.Generic;
using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;

namespace CheeseBot.Extensions
{
    public static class GuildSettingsServiceExtensions
    {
        public static async Task<IReadOnlyList<IPrefix>> GetGuildPrefixesAsync(this GuildSettingsService service, Snowflake guildId)
        {
            var settings = await service.GetGuildSettingsAsync(guildId);
            return settings.Prefixes;
        }

        public static async Task AddPrefixAsync(this GuildSettingsService service, Snowflake guildId, IPrefix prefix)
        {
            var guildSettings = await service.GetGuildSettingsAsync(guildId);
            guildSettings.Prefixes.Add(prefix);
            await service.UpdateGuildSettingsAsync(guildSettings);
        }

        public static async Task RemovePrefixAsync(this GuildSettingsService service, Snowflake guildId, IPrefix prefix)
        {
            var guildSettings = await service.GetGuildSettingsAsync(guildId);
            guildSettings.Prefixes.Remove(prefix);
            await service.UpdateGuildSettingsAsync(guildSettings);
        }

        public static async Task<bool> GuildIsPermittedAsync(this GuildSettingsService service, Snowflake guildId)
        {
            var settings = await service.GetGuildSettingsAsync(guildId);
            return settings.IsPermitted;
        }

        public static async Task SetPermittedStateAsync(this GuildSettingsService service, Snowflake guildId, bool state)
        {
            var settings = await service.GetGuildSettingsAsync(guildId);
            settings.IsPermitted = state;
            await service.UpdateGuildSettingsAsync(settings);
        }
    }
}