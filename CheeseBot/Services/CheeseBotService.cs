using Disqord.Logging;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public abstract class CheeseBotService : ILogging
    {
        public CheeseBotService(ILogger logger)
        {
            Logger = logger;
        }
        public ILogger Logger { get; }
    }
}