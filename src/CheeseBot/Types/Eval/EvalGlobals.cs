namespace CheeseBot.Eval
{
    public class EvalGlobals
    {
        public virtual DiscordCommandContext Context { get; }
        public EvalGlobals(DiscordCommandContext context)
        {
            Context = context;
        }
        
        public DiscordResponseCommandResult Response(string content)
            => Response(new LocalMessage().WithContent(content));

        public DiscordResponseCommandResult Response(params LocalEmbed[] embeds)
            => Response(new LocalMessage().WithEmbeds(embeds));

        public DiscordResponseCommandResult Response(string content, params LocalEmbed[] embeds)
            => Response(new LocalMessage().WithContent(content).WithEmbeds(embeds));

        public DiscordResponseCommandResult Response(LocalMessage message)
        {
            message.AllowedMentions ??= LocalAllowedMentions.None;
            return new DiscordResponseCommandResult(Context, message);
        }

    }
}