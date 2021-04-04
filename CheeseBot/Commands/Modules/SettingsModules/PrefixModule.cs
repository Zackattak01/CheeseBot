using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Group("prefix")]
    public class PrefixModule : DiscordGuildModuleBase
    {
        private readonly GuildSettingsService _guildSettingsService;
        
        public PrefixModule(GuildSettingsService guildSettingsService)
        {
            _guildSettingsService = guildSettingsService;
        }

        [Command]
        public async Task DisplayPrefixAsync()
        {
            var prefixes = await _guildSettingsService.GetGuildPrefixesAsync(Context.GuildId);
            var responseString = "My prefixes for this guild are:\n" + string.Join('\n', prefixes);
            
            await Response(responseString);
        }
    }
}