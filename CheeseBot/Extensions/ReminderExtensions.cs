using CheeseBot.EfCore.Entities;
using Disqord;

namespace CheeseBot.Extensions
{
    public static class ReminderExtensions
    {
        public static LocalEmbed GetEmbed(this Reminder reminder)
        { 
            var messageLink = Discord.MessageJumpLink(reminder.GuildId, reminder.ChannelId, reminder.OriginalMessageId);
            var linkMarkdown = Markdown.Link("Original Message", messageLink);
            return new LocalEmbed()
                .WithTitle("Reminder")
                .WithDescription($"{reminder.Value}\n\n{linkMarkdown}")
                .WithColor(Global.DefaultEmbedColor);
        }
    }
}