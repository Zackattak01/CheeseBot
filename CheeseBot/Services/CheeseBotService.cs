using Disqord.Logging;

namespace CheeseBot.Services
{
    public abstract class CheeseBotService : ILogging
    {
        protected CheeseBotService(ILogger logger)
        {
            Logger = logger;
        }
        public ILogger Logger { get; }
    }
}