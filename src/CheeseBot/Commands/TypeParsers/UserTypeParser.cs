namespace CheeseBot.Commands.TypeParsers
{
    public class UserTypeParser : DiscordGuildTypeParser<IUser>
    {
        public override async ValueTask<TypeParserResult<IUser>> ParseAsync(Parameter parameter, string value, DiscordGuildCommandContext context)
        {
            if (!Snowflake.TryParse(value, out var id) || await context.Bot.FetchUserAsync(id) is not { } user)
                return Failure("Please enter a valid user id.");

            return Success(user);
        }
    }
}