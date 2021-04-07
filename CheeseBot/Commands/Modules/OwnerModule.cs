using System;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class OwnerModule : DiscordModuleBase
    {
        [Command("shutdown", "stop", "die", "kill")]
        [Description("Shuts down and does not restart it")]
        public void Shutdown()
        {  
            //TODO: log out
            //cant find the methods rn
            // Context.Bot.StoppingToken.ca
            Environment.Exit(0);
        }
        
        [Command("restart", "update")]
        [Description("Shuts down and does restarts it")]
        public void Restart()
        {  
            //TODO: log out
            //cant find the methods rn
            Environment.Exit(1);
        }
    }
}