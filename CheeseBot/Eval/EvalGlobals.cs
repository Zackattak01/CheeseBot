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
        
        public DiscordResponseCommandResult Response(string content, LocalMentionsBuilder mentions = null)
            => Response(content, null, mentions);

        public DiscordResponseCommandResult Response(LocalEmbedBuilder embed)
            => Response(null, embed, null);

        public DiscordResponseCommandResult Response(string content, LocalEmbedBuilder embed, LocalMentionsBuilder mentions = null)
            => Response(new LocalMessageBuilder()
                .WithContent(content)
                .WithEmbed(embed)
                .WithMentions(mentions ?? LocalMentionsBuilder.None)
                .Build());

        public DiscordResponseCommandResult Response(LocalMessage message)
            => new(Context, message);

    }
}