using System.Threading.Tasks;
using CheeseBot.EfCore.Entities;
using CheeseBot.Services;
using Disqord.Bot;

namespace CheeseBot.Commands
{
    public abstract class GuildSettingsModule : DiscordGuildModuleBase
    {
        public GuildSettingsService GuildSettingsService { get; set; }

        protected GuildSettings CurrentGuildSettings { get; private set; }
        
        protected override async ValueTask BeforeExecutedAsync()
        {
            CurrentGuildSettings = await GuildSettingsService.GetGuildSettingsAsync(Context.GuildId);
        }
    }
}