using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CheeseBot.Settings;
using CheeseBot.Settings.Formatters;
using Disqord;
using Disqord.Bot;

namespace CheeseBot.EfCore.Entities
{
    public sealed class GuildSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Snowflake GuildId { get; set; }

        [SettingsProperty<PermittedFormatter>("Permitted", 0)]
        public bool IsPermitted { get; set; } = false;
        
        [SettingsProperty<PrefixFormatter>("Prefixes", 1)]
        public List<IPrefix> Prefixes { get; }

        public GuildSettings(Snowflake guildId, List<IPrefix> prefixes)
        {
            GuildId = guildId;
            Prefixes = prefixes;
        }
        
    }
}