using System;
using CheeseBot.EfCore.Entities;

namespace CheeseBot.Extensions
{
    public static class UserStopwatchExtensions
    {
        public static void Stop(this UserStopwatch sw)
            => sw.EndDate = DateTime.Now;
    }
}