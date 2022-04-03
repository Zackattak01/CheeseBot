namespace CheeseBot.Extensions
{
    public static class GuildSettingsExtensions
    {
        public static string GetFormattedPrefixList(this GuildSettings settings)
        {
            var formattedPrefixList = new List<string>(settings.Prefixes.Count);

            foreach (var prefix in settings.Prefixes)
            {
                switch (prefix)
                {
                    case MentionPrefix:
                        formattedPrefixList.Insert(0, prefix.ToString());
                        break;
                    case StringPrefix:
                        formattedPrefixList.Add(Markdown.Code(prefix.ToString()));
                        break;
                }
            }

            return string.Join(", ", formattedPrefixList);
        }
    }
}