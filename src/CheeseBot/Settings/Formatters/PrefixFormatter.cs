namespace CheeseBot.Settings.Formatters
{
    public class PrefixFormatter : ISettingsFormatter
    {
        public static string Format(GuildSettings settings)
            => settings.GetFormattedPrefixList();
    }
}