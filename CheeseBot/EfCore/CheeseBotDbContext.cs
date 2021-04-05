using System.Collections.Generic;
using System.Linq;
using CheeseBot.EfCore.Entities;
using Disqord;
using Disqord.Bot;
using Disqord.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CheeseBot.EfCore
{
    public class CheeseBotDbContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings { get; set; }

        public CheeseBotDbContext(DbContextOptions<CheeseBotDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<GuildSettings>(modelBuilder =>
            {
                modelBuilder.Property(x => x.GuildId)
                    .HasConversion(
                        static snowflake => (ulong) snowflake, 
                        static @ulong => new Snowflake(@ulong));
                modelBuilder.Property(x => x.Prefixes)
                    .HasConversion(
                        static prefixes => prefixes.Select(x => x.ToString()).ToArray(),
                        static arr => new HashSet<IPrefix>(arr.Select(StringToPrefix)));
            });


        }

        private static IPrefix StringToPrefix(string prefix)
        {
            if (Mention.TryParseUser(prefix, out var result))
                return new MentionPrefix(result);
            else
                return new StringPrefix(prefix);
        }
        
        // private static string PrefixToString(IPrefix prefix)
        // {
        //     return prefix.ToString();
        // }
        //
        // private static IPrefix StringToPrefix(string str)
        // {
        //     if (Mention.TryParseUser(str, out var result))
        //         return new MentionPrefix(result);
        //     else
        //         return new StringPrefix(str);
        // }

        
    }
    
}