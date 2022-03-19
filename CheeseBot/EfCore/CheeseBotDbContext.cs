using System.Collections.Generic;
using System.Linq;
using CheeseBot.EfCore.Entities;
using CheeseBot.Extensions;
using Disqord;
using Disqord.Bot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CheeseBot.EfCore
{
    public class CheeseBotDbContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<UserStopwatch> Stopwatches { get; set; }

        public CheeseBotDbContext(DbContextOptions<CheeseBotDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var snowflakeConverter = new ValueConverter<Snowflake, ulong>(
                static snowflake => snowflake,
                static @ulong => new Snowflake(@ulong));

            modelBuilder.UseValueConverterForType<Snowflake>(snowflakeConverter);
            
            modelBuilder.Entity<GuildSettings>(modelBuilder =>
            {
                modelBuilder.Property(x => x.Prefixes)
                    .HasPostgresArrayConversion(
                        static prefix => prefix.ToString(),
                        static str => StringToPrefix(str));
                // .HasConversion(
                // static prefixes => prefixes.Select(x => x.ToString()).ToArray(),
                // static arr => new HashSet<IPrefix>(arr.Select(StringToPrefix)));
            });
        }

        private static IPrefix StringToPrefix(string prefix)
        {
            if (Mention.TryParseUser(prefix, out var result))
                return new MentionPrefix(result);
            else
                return new StringPrefix(prefix);
        }
    }
    
}