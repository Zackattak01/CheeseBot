using System.Threading.Tasks;
using Disqord;
using Disqord.Hosting;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class UnobservedTaskExceptionService : DiscordClientService
    {
        public UnobservedTaskExceptionService(ILogger<UnobservedTaskExceptionService> logger, DiscordClientBase client) : base(logger, client)
        {
            // only raised on a garbage collection
            TaskScheduler.UnobservedTaskException += 
                (_, e) => Logger.LogError($"Unobserved observed task exception:\n {e.Exception.InnerException}");
            
            Logger.LogInformation("Unobserved task exception handling setup.");
        }
    }
}