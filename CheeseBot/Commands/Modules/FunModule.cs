using System;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class FunModule : DiscordModuleBase
    {
        private readonly Random _random;
        
        public FunModule(Random random)
        {
            _random = random;
        }

        [Command("choice", "choose", "decide")]
        [Description("Chooses between any number of specified options")]
        public DiscordCommandResult ChooseAsync([Minimum(2)] params string[] choices)
            => Response(choices[_random.Next(0, choices.Length)]);
        
    }
}