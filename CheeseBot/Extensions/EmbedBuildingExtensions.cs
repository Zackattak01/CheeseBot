using System;
using Disqord;

namespace CheeseBot.Extensions
{
    public static class EmbedBuildingExtensions
    {

        
        public static LocalEmbedBuilder WithDefaultColor(this LocalEmbedBuilder eb)
            => eb.WithColor(Global.DefaultEmbedColor);
        
        public static LocalEmbedBuilder FillLineWithEmptyFields(this LocalEmbedBuilder eb)
        {
            var currentInlineFieldCount = 0;
            
            for (var i = eb.Fields.Count - 1; i >= 0; i--)
            {
                if (!eb.Fields[i].IsInline)
                    break;

                currentInlineFieldCount++;
            }

            // line is already full
            if (currentInlineFieldCount % 3 == 0)
                return eb;

            while (currentInlineFieldCount % 3 != 0)
            {
                eb.AddInlineBlankField();
                currentInlineFieldCount++;
            }

            return eb;
        }

        public static LocalEmbedBuilder AddInlineField(this LocalEmbedBuilder eb, string name, string value)
            => eb.AddField(name, value, true);
        
        public static LocalEmbedBuilder AddInlineField(this LocalEmbedBuilder eb, string name, object value)
            => eb.AddField(name, value, true);
        
        public static LocalEmbedBuilder AddInlineField(this LocalEmbedBuilder eb, LocalEmbedFieldBuilder efb)
            => eb.AddField(efb);
        
        public static LocalEmbedBuilder AddInlineField(this LocalEmbedBuilder eb, Action<LocalEmbedFieldBuilder> action)
            => eb.AddField(action);
        
        public static LocalEmbedBuilder AddInlineBlankField(this LocalEmbedBuilder eb)
            => eb.AddBlankField(true);
    }
}