namespace CheeseBot.Commands.Modules
{
    [Description("All kinds of fun things")]
    public class FunModule : DiscordModuleBase
    {
        private static readonly IReadOnlyList<string> EightBallResponses = new[]
        {
            // good
            "It is certain.",
            "It is decidedly so.",
            "Without a doubt.",
            "Yes – definitely.",
            "You may rely on it.",
            "As I see it, yes.",
            "Most likely.",
            "Outlook good.",
            "Yes.",
            "Signs point to yes.",
            // uncertain
            "Reply hazy, try again.",
            "Ask again later.",
            "Better not tell you now.",
            "Cannot predict now.",
            "Concentrate and ask again.",
            // bad
            "Don't count on it.",
            "My reply is no.",
            "My sources say no.",
            "Outlook not so good.",
            "Very doubtful. "
        };
        
        private readonly Random _random;
        
        public FunModule(Random random)
        {
            _random = random;
        }

        [Command("choice", "choose", "decide")]
        [Description("Chooses between any number of specified options")]
        public DiscordCommandResult Choose([Minimum(2)] params string[] choices)
            => Response($"I choose {Markdown.Code(choices[_random.Next(0, choices.Length)])}");

        [RequireGuild]
        [Command("pp")]
        [Description(":eyes:")]
        public DiscordCommandResult PP(IMember user = null)
        {
            user ??= Context.Author as IMember;
            return Response($"{user!.Mention}'s pp {user.Id % 13}in");
        }

        [Command("random", "rand")]
        [Description("Gets the next number I can find laying around")]
        public DiscordCommandResult Random()
            => Response($"Your random number is: {_random.Next()}");

        [Command("random", "rand")]
        [Description("Gets the next number I can find laying around with a max value")]
        public DiscordCommandResult Random([Minimum(0)] int max)
            => Response($"Your random number with a maximum of {max} is: {_random.Next(max + 1)}");
        
        [Command("random", "rand")]
        [Description("Your so needy.  Fine.  Ill get the next number with between a min and a max.")]
        public DiscordCommandResult Random(int min, int max)
        {
            if (min > max)
                return Response("Seriously? Obviously the minimum has to be less than the maximum :rolling_eyes:");
            return Response($"Your random number between {min} and {max} is: {_random.Next(min, max + 1)}");
        }
        
        [RequireGuild]
        [Command("vote", "poll")]
        [Description("A poll.  You only use this command because it has an embed.")]
        public async Task VoteAsync([Remainder] string poll)
        {
            if (Context is not DiscordGuildCommandContext guildContext)
            {
                await Response("This is why we cant have nice things.  Report this immediately or else...");
                return;
            }
            
            await Context.Message.DeleteAsync();
            var e = new LocalEmbed()
                .WithDefaultColor()
                .WithAuthor($"{guildContext.Author.GetDisplayName()}'s Poll", guildContext.Author.GetAvatarUrl())
                .WithDescription(poll);
            
            var msg = await Response(e);
            await msg.AddReactionAsync(new LocalEmoji("👍"));
            await msg.AddReactionAsync(new LocalEmoji("👎"));
        }

        [Command("8ball")]
        [Description("My word is final.")]
        public DiscordCommandResult EightBall([Remainder] string question)
            => Response(EightBallResponses[_random.Next(0, EightBallResponses.Count)]);

        [Command("coinflip", "flip", "coin")]
        [Description("Flip a virtual coin")]
        public DiscordCommandResult CoinFlip()
            => Response(_random.Next(0, 2) == 0 ? "Heads" : "Tails");
    }
}