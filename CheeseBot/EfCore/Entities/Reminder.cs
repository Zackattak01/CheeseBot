using System;
using System.ComponentModel.DataAnnotations;
using CheeseBot.Extensions;
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

        public string GetTimeString()
        {
            var article = ExecutionTime.Date == DateTime.Today ? "at" : "on";
            return $"{article} {ExecutionTime.Humanize()}";
        }
        
        private string GetTimeCapitalizeFirstLetter()
        {
            var time = GetTimeString();
            return char.ToUpper(time[0]) + time[1..];
        }

        public override string ToString()
            => $"{GetTimeCapitalizeFirstLetter()}: {Value}";
    }
}