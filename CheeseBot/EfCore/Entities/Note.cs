using System;
using System.ComponentModel.DataAnnotations;
using Disqord;

namespace CheeseBot.EfCore.Entities
{
    public class Note
    {
        private const string TodayFormatString = "a\\t h:mm tt";
        private const string DateFormatString = "on MM/dd/yy a\\t h:mm tt";
        
        [Key]
        public int Id { get; set; }
        
        public Snowflake OwnerId { get; set; }
        
        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public Note(Snowflake ownerId, string content, DateTime createdOn)
        {
            OwnerId = ownerId;
            Content = content;
            CreatedOn = createdOn;
        }

        private string GetTimeString()
        {
            var returnString = CreatedOn.ToString(CreatedOn.Date == DateTime.Now.Date ? TodayFormatString : DateFormatString);

            return returnString;
        }

        public override string ToString()
        {
            return $"{Content} (Created {GetTimeString()})";
        }
    }
}