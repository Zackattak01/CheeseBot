using System.Text;

namespace CheeseBot.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static string DeCamelCase(this string s)
        {
            const char space = ' ';
            var stringBuilder = new StringBuilder();
            
            var lastWordIndex = 0;
            for (var i = 0; i < s.Length; i++)
            {
                var isUpperCase = char.IsUpper(s[i]);
                var isLastChar = i + 1 == s.Length;

                if (isUpperCase)
                {
                    stringBuilder.Append(s[lastWordIndex..i]).Append(space);
                    lastWordIndex = i;
                }
                
                if (isLastChar)
                {
                    stringBuilder.Append(s[lastWordIndex..s.Length]).Append(space);
                }
            }

            return stringBuilder.ToString().Trim();
        }

        public static string HumanTruncateAt(this string s, int maxLength)
        {
            const string truncationIndicator = "...";
            
            if (!(s.Length > maxLength))
                return s;

            return s[..(maxLength - 3)] + truncationIndicator;
        }
    }
}