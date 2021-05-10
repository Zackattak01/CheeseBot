namespace CheeseBot.GitHub
{
    public abstract class GitHubLineSelection : IGitHubLineSelection
    {
        private const char MultiLineSeparatorChar = '-';
        private const char LineIndicatorChar = 'L';

        public abstract string TransformContent(string fullContent);

        public static bool TryParse(string input, out GitHubLineSelection selection)
        {
            selection = null;
            var lines = input.Split(MultiLineSeparatorChar);

            return lines.Length switch
            {
                1 => TryParseSingleLine(lines[0], out selection),
                2 => TryParseMultiLine(lines, out selection),
                _ => false
            };
        }
        
        private static bool TryParseSingleLine(string line, out GitHubLineSelection singleLineSelection)
        {
            singleLineSelection = null;

            if (line.Length < 2)
                return false;
            else if (line[0] != LineIndicatorChar)
                return false;

            if (!int.TryParse(line[1..], out var selectedLine))
                return false;

            singleLineSelection = new GitHubSingleLineSelection(selectedLine);
            return true;
        }

        private static bool TryParseMultiLine(string[] lines, out GitHubLineSelection multiLineSelection)
        {
            multiLineSelection = null;
            
            if (lines[0].Length < 2 || lines[1].Length < 2)
                return false;
            else if (lines[0][0] != LineIndicatorChar || lines[1][0] != LineIndicatorChar)
                return false;

            if (!int.TryParse(lines[0][1..], out var startLine) && startLine > 0)
                return false; 
            
            if (!int.TryParse(lines[1][1..], out var endLine) && endLine >= startLine)
                return false;

            multiLineSelection = new GitHubMultiLineSelection(startLine, endLine);
            return true;
        }
        
    }
}