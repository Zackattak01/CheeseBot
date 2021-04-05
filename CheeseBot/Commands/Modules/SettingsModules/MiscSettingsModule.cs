using System.Threading.Tasks;
using CheeseBot.Extensions;
using CheeseBot.Services;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class MiscSettingsModule : GuildSettingsModule
    {
        [Command("permit")]
        [RequireBotOwner]
        public async Task<DiscordCommandResult> PermitAsync()
        {
            if (!Settings.IsPermitted)
            {
                await GuildSettingsService.SetPermittedStateAsync(Context.GuildId, true);
                return Response("This guild has been permitted!");
            }
            else
                return Response("This guild is already permitted!");
            
        }

        [Command("unpermit")]
        [RequireBotOwner]
        public async Task<DiscordCommandResult> UnpermitAsync()
        {
            if (Settings.IsPermitted)
            {
                await GuildSettingsService.SetPermittedStateAsync(Context.GuildId, false);
                return Response("This guild has been unpermitted!");
            }
            else
                return Response("This guild is not permitted!");
        }
    }
}