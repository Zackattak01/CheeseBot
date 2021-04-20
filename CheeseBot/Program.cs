using System;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Disqord;
using CheeseBot.EfCore;
using CheeseBot.Extensions;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace CheeseBot
{
    class Program
    {
        private const string ConfigPath = "./config.json";
        
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureLogging(x =>
                {
                    var logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                        .CreateLogger();
                    
                    x.AddSerilog(logger, true);
                    
                    x.Services.Remove(x.Services.First(x => x.ServiceType == typeof(ILogger<>)));
                    x.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                })
                .ConfigureServices((context, services) =>
                {
                    var connString = context.Configuration["postgres:connection_string"];
                    services.AddDbContext<CheeseBotDbContext>(x => x.UseNpgsql(connString).UseSnakeCaseNamingConvention());
                    services.AddPrefixProvider<CheeseBotPrefixProvider>();
                    services.AddSingleton<Random>();//TODO: Cryptographic random... just for fun
                    services.AddCheeseBotServices();
                })
                .ConfigureAppConfiguration(configuration => configuration.AddJsonFile(ConfigPath))
                .ConfigureDiscordBot<CheeseDisqordBot>((context, bot) =>
                {
                    bot.Token = context.Configuration["discord:token"];
                    bot.OwnerIds = new[] {new Snowflake(Global.AuthorId)};
                    bot.Intents = GatewayIntents.All;
                    
                })
                .Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var context = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();
                logger.LogInformation("Migrating database....");
                await context.Database.MigrateAsync();
                logger.LogInformation("Done migrating database.");
            }
            
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
