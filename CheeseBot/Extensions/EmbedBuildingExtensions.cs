namespace CheeseBot.Extensions
{
    public static class EmbedBuildingExtensions
    {
        public static LocalEmbed WithDefaultColor(this LocalEmbed e)
            => e.WithColor(Global.DefaultEmbedColor);
        
        public static LocalEmbed FillLineWithEmptyFields(this LocalEmbed e)
        {
            var currentInlineFieldCount = 0;
            
            for (var i = e.Fields.Count - 1; i >= 0; i--)
            {
                if (!e.Fields[i].IsInline)
                    break;

                currentInlineFieldCount++;
            }

            // line is already full
            if (currentInlineFieldCount % 3 == 0)
                return e;

            while (currentInlineFieldCount % 3 != 0)
            {
                e.AddInlineBlankField();
                currentInlineFieldCount++;
            }

            return e;
        }

        public static LocalEmbed AddInlineField(this LocalEmbed e, string name, string value)
            => e.AddField(name, value, true);
        
        public static LocalEmbed AddInlineField(this LocalEmbed e, string name, object value)
            => e.AddField(name, value, true);
        
        public static LocalEmbed AddInlineField(this LocalEmbed e, LocalEmbedField ef)
            => e.AddField(ef);

        public static LocalEmbed AddInlineBlankField(this LocalEmbed e)
            => e.AddBlankField(true);
    }
}