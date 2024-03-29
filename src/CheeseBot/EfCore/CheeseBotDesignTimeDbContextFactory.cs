#if DEBUG
using Microsoft.EntityFrameworkCore.Design;

namespace CheeseBot.EfCore
{
    public class CheeseBotDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CheeseBotDbContext>
    {
        public CheeseBotDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CheeseBotDbContext>();
            builder.UseNpgsql("Host=localhost;Database=cheesebot;Username=postgres;Password=password").UseSnakeCaseNamingConvention();
            return new CheeseBotDbContext(builder.Options);
        }
    }
}
#endif