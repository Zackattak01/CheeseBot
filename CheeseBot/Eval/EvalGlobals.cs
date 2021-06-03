using Disqord;
using Disqord.Bot;

namespace CheeseBot.Eval
{
    public class EvalGlobals
    {
        public virtual DiscordCommandContext Context { get; }
        public EvalGlobals(DiscordCommandContext context)
        {
            Context = context;
        }
        
        public DiscordResponseCommandResult Response(string content, LocalAllowedMentions mentions = null)
            => Response(content, null, mentions);

        public DiscordResponseCommandResult Response(LocalEmbed embed)
            => Response(null, embed);

        public DiscordResponseCommandResult Response(string content, LocalEmbed embed, LocalAllowedMentions mentions = null)
            => Response(new LocalMessage()
                .WithContent(content)
                .WithEmbed(embed)
                .WithAllowedMentions(mentions ?? LocalAllowedMentions.None));

        public DiscordResponseCommandResult Response(LocalMessage message)
            => new(Context, message);

    }
}