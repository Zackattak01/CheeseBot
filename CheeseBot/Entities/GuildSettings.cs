using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Disqord.Bot;

namespace CheeseBot.Entities
{
    public sealed class GuildSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong GuildId { get; set; }

        public HashSet<string> PrefixStrings { get; set; }
        
        [NotMapped]
        public HashSet<IPrefix> Prefixes { get; }

        public GuildSettings()
        {
            Prefixes = new HashSet<IPrefix>(PrefixStrings.Select(x => new StringPrefix(x)));
        }

        public GuildSettings(ulong guildId, HashSet<IPrefix> prefixes)
        {
            Prefixes = prefixes;

            PrefixStrings = new HashSet<string>(prefixes.Where(x => x is StringPrefix).Select(x => x.ToString()));
        }
    }
}