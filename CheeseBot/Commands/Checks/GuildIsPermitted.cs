using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord.Bot;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace CheeseBot.Commands.Checks
{
    public class GuildIsPermitted : DiscordGuildCheckAttribute
    {
        

        public override async ValueTask<CheckResult> CheckAsync(DiscordGuildCommandContext context)
        {
            var guildSettingsService = context.Services.GetRequiredService<GuildSettingsService>();
            var isPermitted = await guildSettingsService.GuildIsPermittedAsync(context.GuildId);
            return isPermitted ? Success() : Failure("Your guild is not permitted.");
        }
    }
}