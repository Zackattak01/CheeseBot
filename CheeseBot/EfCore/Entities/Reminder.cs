using System;
using System.ComponentModel.DataAnnotations;
using Disqord;

namespace CheeseBot.EfCore.Entities
{
    public class Reminder
    {
        [Key]
        public int Id { get; set; }

        public DateTime ExecutionTime { get; set; }

        public Snowflake UserId { get; set; }

        public Snowflake ChannelId { get; set; }

        public Snowflake GuildId { get; set; }

        public Snowflake OriginalMessageId { get; set; }

        public string Value { get; set; }

        public Reminder(DateTime executionTime, Snowflake userId, Snowflake channelId, Snowflake guildId, Snowflake originalMessageId, string value)
        {
            ExecutionTime = executionTime;
            UserId = userId;
            ChannelId = channelId;
            GuildId = guildId;
            OriginalMessageId = originalMessageId;
            Value = value;
        }

        public string FormatMarkdownTimestamp() 
            => ExecutionTime.Date == DateTime.Today
                ? Markdown.Timestamp(ExecutionTime, Markdown.TimestampFormat.RelativeTime)
                : Markdown.Timestamp(ExecutionTime);

        public override string ToString()
            => $"{FormatMarkdownTimestamp()}: {Value}";
    }
}