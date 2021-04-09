using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.TypeReaders
{
    public class PrefixTypeReader : DiscordTypeParser<IPrefix>
    {
        public override ValueTask<TypeParserResult<IPrefix>> ParseAsync(Parameter parameter, string value, DiscordCommandContext context)
        {
            IPrefix prefix;
            if (Mention.TryParseUser(value, out var result))
                prefix = new MentionPrefix(result);
            else
                prefix = new StringPrefix(value);

            return Success(prefix);
        }
    }
}