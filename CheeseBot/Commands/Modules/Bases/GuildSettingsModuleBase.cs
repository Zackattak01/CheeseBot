using System.Threading.Tasks;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using CheeseBot.Services;
using Disqord.Bot;

namespace CheeseBot.Commands.Modules
{
    public abstract class GuildSettingsModuleBase : DiscordGuildModuleBase
    {
        public GuildSettingsService GuildSettingsService { get; set; }

        public CheeseBotDbContext DbContext { get; set; }

        protected GuildSettings CurrentGuildSettings { get; private set; }
        
        protected override async ValueTask BeforeExecutedAsync()
        {
            CurrentGuildSettings = await GuildSettingsService.GetGuildSettingsAsync(Context.GuildId);
        }

        protected override async ValueTask AfterExecutedAsync()
        {
            DbContext.Update(CurrentGuildSettings);
            await DbContext.SaveChangesAsync();
        }
    }
}