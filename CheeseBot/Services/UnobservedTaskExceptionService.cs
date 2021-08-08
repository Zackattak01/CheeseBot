using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class UnobservedTaskExceptionService : CheeseBotService
    {
        public UnobservedTaskExceptionService(ILogger<UnobservedTaskExceptionService> logger) : base(logger)
        {
            // only raised on a garbage collection
            TaskScheduler.UnobservedTaskException += 
                (_, e) => Logger.LogError($"Unobserved task exception:\n {e.Exception.InnerException}");
            
            Logger.LogInformation("Unobserved task exception handling setup.");
        }
    }
}