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

        [Command("random", "rand")]
        public DiscordCommandResult Random()
            => Response($"Your random number is: {_random.Next()}");

        [Command("random", "rand")]
        public DiscordCommandResult Random([Minimum(0)] int max)
            => Response($"Your random number with a maximum of {max} is: {_random.Next(max + 1)}");
        
        [Command("random", "rand")]
        public DiscordCommandResult Random(int min, int max)
        {
            if (min > max)
                return Response("Seriously? Obviously the minimum has to be less than the maximum :rolling_eyes:");
            return Response($"Your random number between {min} and {max} is: {_random.Next(min, max + 1)}");
        }
    }
}