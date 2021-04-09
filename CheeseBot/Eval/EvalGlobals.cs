using Disqord.Bot;

namespace CheeseBot.Eval
{
    public class EvalGlobals
    {
        public DiscordCommandContext Context { get; }
        public EvalGlobals(DiscordCommandContext context)
        {
            Context = context;
        }
    }
}