using System;
using System.Collections.Generic;
using System.Linq;
using CheeseBot.Entities;
using Disqord.Bot;
using Disqord.Rest.Api;
using Microsoft.EntityFrameworkCore;
using Qmmands;

namespace CheeseBot.EfCore
{
    public class CheeseBotDbContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings { get; set; }

        public CheeseBotDbContext(DbContextOptions<CheeseBotDbContext> options)
            : base(options) { }
    }
    
}