using System.Threading;
using CheeseBot.Commands.TypeParsers;
using Microsoft.Extensions.Options;

namespace CheeseBot.Disqord
{
    //screwed myself over with the name of project lol
    public class CheeseDisqordBot : DiscordBot
    { 
        public CheeseDisqordBot(
            IOptions<DiscordBotConfiguration> options,
            ILogger<DiscordBot> logger,
            IServiceProvider services,
            DiscordClient client)
            : base(options, logger, services, client) { }

        protected override ValueTask AddTypeParsersAsync(CancellationToken cancellationToken = default)
        {
            Commands.AddTypeParser(new PrefixTypeParser());
            Commands.AddTypeParser(new DateTimeTypeParser());
            Commands.AddTypeParser(new ReminderTypeParser());
            Commands.AddTypeParser(new UserTypeParser());
            return base.AddTypeParsersAsync(cancellationToken);
        }
    }
}