using System.Threading.Tasks;
using CheeseBot.Extensions;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class MiscSettingsModule : GuildSettingsModule
    {
        [Command("permit")]
        [RequireBotOwner]
        public DiscordCommandResult PermitAsync()
        {
            if (!CurrentGuildSettings.IsPermitted)
            {
                CurrentGuildSettings.IsPermitted = true;
                return Response("This guild has been permitted!");
            }
            else
                return Response("This guild is already permitted!");
            
        }

        [Command("unpermit")]
        [RequireBotOwner]
        public DiscordCommandResult UnpermitAsync()
        {
            if (CurrentGuildSettings.IsPermitted)
            {
                CurrentGuildSettings.IsPermitted = false;
                return Response("This guild has been unpermitted!");
            }
            else
                return Response("This guild is not permitted!");
        }
    }
}