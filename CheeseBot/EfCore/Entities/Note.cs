using System.ComponentModel.DataAnnotations;

namespace CheeseBot.EfCore.Entities
{
    public class Note
    {
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

        public override string ToString()
        {
            var article = CreatedOn.Date == DateTime.Today ? "at" : "on";
            return $"{Content} (Created {article} {CreatedOn.Humanize()})";
        }
    }
}