using System;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.EfCore.Entities;
using Disqord.Bot;
using HumanTimeParser.Core.Parsing;
using HumanTimeParser.English;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace CheeseBot.Commands.TypeParsers
{
    public class ReminderTypeParser : DiscordGuildTypeParser<Reminder>
    {
        public override ValueTask<TypeParserResult<Reminder>> ParseAsync(Parameter parameter, string value, DiscordGuildCommandContext context)
        {
            var result = context.Services.GetRequiredService<EnglishTimeParser>().Parse(value);

            if (result is not ISuccessfulTimeParsingResult<DateTime> successfulResult)
                return Failure((result as IFailedTimeParsingResult)!.ErrorReason);

            var reminderValue = successfulResult.LastParsedTokenIndex == value.Length ?
                value[..successfulResult.FirstParsedTokenIndex] : 
                value[successfulResult.LastParsedTokenIndex..];

            var reminder = new Reminder(
                    successfulResult.Value,
                    context.Author.Id,
                    context.ChannelId,
                    context.GuildId,
                    context.Message.Id,
                    reminderValue.Trim()
                );

                return new ValueTask<TypeParserResult<Reminder>>(new TypeParserResult<Reminder>(reminder));
        }
    }
}