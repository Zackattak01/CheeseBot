using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Qmmands;

namespace CheeseBot.Commands.TypeParsers
{
    public class UserTypeParser : DiscordGuildTypeParser<IUser>
    {
        public override async ValueTask<TypeParserResult<IUser>> ParseAsync(Parameter parameter, string value, DiscordGuildCommandContext context)
        {
            if (!Snowflake.TryParse(value, out var id))
                return Failure("Please enter a valid discord id.");
            
            return Success(await context.Bot.FetchUserAsync(id));
        }
    }
}