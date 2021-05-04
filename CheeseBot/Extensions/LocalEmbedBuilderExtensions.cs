using Disqord;

namespace CheeseBot.Extensions
{
    public static class LocalEmbedBuilderExtensions
    {
        public static LocalEmbedBuilder WithDefaultColor(this LocalEmbedBuilder eb)
            => eb.WithColor(Global.DefaultEmbedColor);
    }
}