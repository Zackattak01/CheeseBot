namespace CheeseBot.Settings.Formatters
{
    public class PermittedFormatter : ISettingsFormatter
    {
        public static string Format(GuildSettings settings)
        {
            return settings.IsPermitted ? true.ToString() : null;
        }
    }
}