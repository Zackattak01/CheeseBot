using System;
using Disqord;
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
        public DiscordCommandResult Choose([Minimum(2)] params string[] choices)
            => Response(choices[_random.Next(0, choices.Length)]);
            
        [Command("pp")]
        public DiscordCommandResult PP(IMember user = null)
        {
            var size = _random.Next(-1, 12);
            var str = size == -1 ? "an inverse peen" : $"{size}in.";
            return Response($"{(user ?? Context.Author).Mention}'s pp is {str}");
        }
    }
}