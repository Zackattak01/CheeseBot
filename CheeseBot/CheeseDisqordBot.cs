using System;
using System.Threading;
using System.Threading.Tasks;
using CheeseBot.Commands.TypeReaders;
using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;

namespace CheeseBot
{
    //screwed myself over with the name of project lol
    public class CheeseDisqordBot : DiscordBot
    {
        public CheeseDisqordBot(IOptions<DiscordBotConfiguration> options, ILogger<DiscordBot> logger,
            IPrefixProvider prefixes, ICommandQueue queue, CommandService commands, IServiceProvider services,
            DiscordClient client) : base(options, logger, prefixes, queue, commands, services, client) { }

        protected override ValueTask AddTypeParsersAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Commands.AddTypeParser(new PrefixTypeReader());
            return base.AddTypeParsersAsync(cancellationToken);
        }
    }
}