using System;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.EfCore.Entities;
using Disqord.Bot;
using HumanTimeParser.Core.Parsing;
using HumanTimeParser.English;
using Qmmands;

namespace CheeseBot.Commands.TypeParsers
{
    public class ReminderTypeParser : DiscordGuildTypeParser<Reminder>
    {
        public override ValueTask<TypeParserResult<Reminder>> ParseAsync(Parameter parameter, string value, DiscordGuildCommandContext context)
        {
            var result = EnglishTimeParser.Parse(value);

            if (result is not ISuccessfulTimeParsingResult<DateTime> successfulResult)
                return Failure((result as IFailedTimeParsingResult)!.ErrorReason);

            var sections = value.Split(' ');

            string reminderValue;
            if (successfulResult.LastParsedTokenIndex == sections.Length - 1)
                reminderValue = string.Join(' ', sections.Take(successfulResult.FirstParsedTokenIndex));
            else
                reminderValue = string.Join(' ', sections.Skip(successfulResult.FirstParsedTokenIndex + 1));
                
            var reminder = new Reminder(
                    successfulResult.Value,
                    context.Author.Id,
                    context.ChannelId,
                    context.GuildId,
                    context.Message.Id,
                    reminderValue
                );

                return new ValueTask<TypeParserResult<Reminder>>(new TypeParserResult<Reminder>(reminder));
        }
    }
}