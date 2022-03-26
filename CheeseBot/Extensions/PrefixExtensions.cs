using System;
using Disqord;
using Disqord.Bot;

namespace CheeseBot.Extensions
{
    public static class PrefixExtensions
    {
        public static string Format(this IPrefix prefix)
        {
            return prefix switch
            {
                StringPrefix => Markdown.Code(prefix),
                MentionPrefix => prefix.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(prefix), prefix, null)
            };
        }
    }
}