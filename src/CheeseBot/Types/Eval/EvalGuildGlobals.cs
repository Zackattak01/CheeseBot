namespace CheeseBot.Eval
{
    public class EvalGuildGlobals : EvalGlobals
    {
        public override DiscordGuildCommandContext Context { get; }

        public EvalGuildGlobals(DiscordGuildCommandContext context) : base(context)
        {
            Context = context;
        }
    }
}