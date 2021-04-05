using System.Threading.Tasks;
using CheeseBot.EfCore.Entities;
using CheeseBot.Services;
using Disqord.Bot;
using Disqord.Rest.Api;

namespace CheeseBot.Commands
{
    public class GuildSettingsModule : DiscordGuildModuleBase
    {
        public GuildSettingsService GuildSettingsService { get; set; }

        protected GuildSettings Settings { get; private set; }
        
        protected override async ValueTask BeforeExecutedAsync()
        {
            Settings = await GuildSettingsService.GetGuildSettingsAsync(Context.GuildId);
        }
    }
}