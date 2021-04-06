using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Disqord;
using Disqord.Bot;

namespace CheeseBot.EfCore.Entities
{
    public sealed class GuildSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Snowflake GuildId { get; set; }

        public bool IsPermitted { get; set; } = false;
        
        public HashSet<IPrefix> Prefixes { get; }//TODO: Readonly set

        public GuildSettings(Snowflake guildId, HashSet<IPrefix> prefixes)
        {
            GuildId = guildId;
            Prefixes = prefixes;
        }
        
    }
}