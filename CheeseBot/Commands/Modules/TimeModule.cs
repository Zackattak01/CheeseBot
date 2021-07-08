using System;
using CheeseBot.Extensions;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Group("time")]
    [Description("Commands for interacting with time")]
    public class TimeModule : DiscordModuleBase
    {
        [Command("until", "since")]
        [Description("Since you cant calculate time.  I'll do it for you.")]
        public DiscordCommandResult TimeDifference([Remainder] DateTime time)
        {
            var now = DateTime.Now;
            if (time < now)
                return Response($"The time since {time.Humanize()} is {(now - time).Humanize()}");
            else if (time > now)
                return Response($"The time until {time.Humanize()} is {(time - now).Humanize()}");
            else if (time == now)
                return Response("Your an idiot... the time you provided is *now*");

            return Response("You broke the spacetime continuum.");
        }
    }
}