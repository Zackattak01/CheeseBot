namespace CheeseBot.Services
{
    public class UnobservedTaskExceptionService : CheeseBotService
    {
        public UnobservedTaskExceptionService(ILogger<UnobservedTaskExceptionService> logger) : base(logger)
        {
            // only raised on a garbage collection
            TaskScheduler.UnobservedTaskException += 
                (_, e) => Logger.LogError(e.Exception.InnerException, "Unobserved task exception:");
            
            Logger.LogInformation("Unobserved task exception handling setup.");
        }
    }
}