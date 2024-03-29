using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheeseBot.EfCore.Entities
{
    public class UserStopwatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Snowflake Id { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }

        [NotMapped]
        public bool IsFinished => EndDate is not null;

        public UserStopwatch()
        {
            
        }

        public UserStopwatch(Snowflake userId)
        {
            Id = userId;
            StartDate = DateTime.Now;
        }

        public void Stop()
            => EndDate = DateTime.Now;

        public override string ToString()
        {
            if (!IsFinished)
                return (DateTime.Now - StartDate).Humanize();
            else
                return (EndDate!.Value - StartDate).Humanize();
        }
    }
}