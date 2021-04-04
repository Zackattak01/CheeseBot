using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Disqord.Bot;

namespace CheeseBot.EfCore.Entities
{
    public sealed class GuildSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong GuildId { get; set; }
        
        public HashSet<IPrefix> Prefixes { get; }

        public GuildSettings(ulong guildId, HashSet<IPrefix> prefixes)
        {
            GuildId = guildId;
            Prefixes = prefixes;
        }
        
    }
}