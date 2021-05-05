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
                eb.AddBlankField(isInline: true);
                currentInlineFieldCount++;
            }

            return eb;
        }
        
        
    }
}