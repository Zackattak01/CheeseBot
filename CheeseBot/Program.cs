using System;
using Disqord.Bot.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
namespace CheeseBot
{
    class Program
    {
        private const string ConfigPath = "./config.json";
        
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog((context, logger) =>
                {
                    logger.ReadFrom.Configuration(context.Configuration, "serilog")
                        .WriteTo.Console();
                })
                .ConfigureServices(services => {})
                .ConfigureAppConfiguration(configuration => configuration.AddJsonFile(ConfigPath))
                .ConfigureDiscordBot((context, bot) =>
                {
                    bot.Token = context.Configuration["discord:token"];
                    bot.Prefixes = new[] { "hey cheeseman", "!" };
                    bot.UseMentionPrefix = true;
                })
                .Build();

            using (host)
            {
                host.Run();
            }
        }
    }
}
