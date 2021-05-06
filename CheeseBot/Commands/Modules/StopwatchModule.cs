using System.Threading.Tasks;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Group("sw", "stopwatch")]
    public class StopwatchModule : DiscordModuleBase
    {
        private readonly CheeseBotDbContext _dbContext;
        
        public StopwatchModule(CheeseBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [Command]
        [Description("View your currently running stopwatch")]
        public async Task<DiscordCommandResult> ViewStopwatchAsync()
        {
            var sw = await FindStopwatchAsync();

            if (sw is null)
                return Response("You do not have a running stopwatch");
            else if (!sw.IsFinished)
                return Response($"Your stopwatch has been running for {sw}");
            else
                return Response($"Your last stopwatch ran for {sw}");
        } 
        
        [Command("start", "new")]
        [Description("Oh? Now I have to keep time for you? Fine. Starts a new reminder.")]
        public async Task<DiscordCommandResult> StartAsync()
        {
            var sw = await FindStopwatchAsync();

            if (sw is not null)
            {
                if (!sw.IsFinished)
                    return Response("You already have a stopwatch running");
                else
                    DeleteStopwatch(sw);
            }

            await CreateStopwatchAsync();

            return Response("Stopwatch started!");
        }
        
        [Command("stop", "end")]
        [Description("Done using me? Good. Stops your stopwatch.")]
        public async Task<DiscordCommandResult> StopAsync()
        {
            var sw = await FindStopwatchAsync();

            if (sw is null || sw.IsFinished)
                return Response("You do not have a stopwatch running.");
            
            sw.Stop();
            _dbContext.Update(sw);

            return Response($"Stopwatch stopped after running for {sw}.");
        }

        [Command("reset", "restart")]
        [Description("Great. Here we go again. I'll restart your stopwatch.  This time.")]
        public async Task<DiscordCommandResult> ResetAsync()
        {
            var sw = await FindStopwatchAsync();

            if (sw is null)
            {
                await CreateStopwatchAsync();
                return Response("Stopwatch reset");
            }

            DeleteStopwatch(sw);
            await CreateStopwatchAsync();
            return Response("Stopwatch reset");
        }

        private async Task CreateStopwatchAsync()
        {
            var sw = new UserStopwatch(Context.Author.Id);
            
            await _dbContext.Stopwatches.AddAsync(sw);
        }

        private ValueTask<UserStopwatch> FindStopwatchAsync()
            => _dbContext.Stopwatches.FindAsync(Context.Author.Id);
        
        private void DeleteStopwatch(UserStopwatch sw)
            => _dbContext.Stopwatches.Remove(sw);

        protected override async ValueTask AfterExecutedAsync()
            => await _dbContext.SaveChangesAsync();
        
    }
}