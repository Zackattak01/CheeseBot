using CheeseBot.EfCore.Entities;

namespace CheeseBot.Settings.Formatters
{
    public interface ISettingsFormatter
    {
        public static abstract string Format(GuildSettings settings);
    }
}