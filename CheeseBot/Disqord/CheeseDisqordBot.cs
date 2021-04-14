using System;
using System.Threading;
using System.Threading.Tasks;
using CheeseBot.Commands.TypeParsers;
using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;

namespace CheeseBot.Disqord
{
    //screwed myself over with the name of project lol
    public class CheeseDisqordBot : DiscordBot
    {
        public CheeseDisqordBot(IOptions<DiscordBotConfiguration> options, ILogger<DiscordBot> logger,
            IPrefixProvider prefixes, ICommandQueue queue, CommandService commands, IServiceProvider services,
            DiscordClient client) : base(options, logger, prefixes, queue, commands, services, client) { }

        protected override ValueTask AddTypeParsersAsync(CancellationToken cancellationToken = new())
        {
            Commands.AddTypeParser(new PrefixTypeParser());
            Commands.AddTypeParser(new DateTimeTypeParser());
            return base.AddTypeParsersAsync(cancellationToken);
        }
    }
}