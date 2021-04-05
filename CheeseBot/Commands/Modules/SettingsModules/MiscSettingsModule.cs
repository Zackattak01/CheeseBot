using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class MiscSettingsModule : DiscordGuildModuleBase
    {
        private readonly GuildSettingsService _guildSettingsService;
        
        public MiscSettingsModule(GuildSettingsService guildSettingsService)
        {
            _guildSettingsService = guildSettingsService;
        }

        [Command("permit")]
        [RequireBotOwner]
        public async Task<DiscordCommandResult> PermitAsync()
        {
            if (!await _guildSettingsService.GuildIsPermittedAsync(Context.GuildId))
            {
                await _guildSettingsService.PermitGuildAsync(Context.GuildId);
                return Response("This guild has been permitted!");
            }
            else
                return Response("This guild is already permitted!");
            
        }
    }
}