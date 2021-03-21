﻿using System.ComponentModel;
using System.Linq;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
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
        
        public static void Main(string[] args)
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
                .ConfigureServices(services => {})
                .ConfigureAppConfiguration(configuration => configuration.AddJsonFile(ConfigPath))
                .ConfigureDiscordBot((context, bot) =>
                {
                    bot.Token = context.Configuration["discord:token"];
                    bot.Prefixes = new[] { "hey cheeseman", "!" }; //TODO: IPrefixProvider
                    bot.UseMentionPrefix = true;
                    bot.OwnerIds = new[] {new Snowflake(Global.OwnerId)};
                    bot.Intents = GatewayIntents.All;
                    
                })
                .Build();

            using (host)
            {
                host.Run();
            }
        }
    }
}
