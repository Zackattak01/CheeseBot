using HumanTimeParser.Core.Parsing;

namespace CheeseBot.Commands.TypeParsers
{
    public class DateTimeTypeParser : DiscordTypeParser<DateTime>
    {
        public override ValueTask<TypeParserResult<DateTime>> ParseAsync(Parameter parameter, string value, DiscordCommandContext context)
        {
            var result = context.Services.GetRequiredService<EnglishTimeParser>().Parse(value);

            if (result is not ISuccessfulTimeParsingResult<DateTime> successfulResult)
                return Failure((result as IFailedTimeParsingResult)?.ErrorReason);

            return Success(successfulResult.Value);
        }
    }
}