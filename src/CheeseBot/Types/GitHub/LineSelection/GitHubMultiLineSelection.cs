namespace CheeseBot.GitHub
{
    public class GitHubMultiLineSelection : GitHubLineSelection
    {
        public int StartLine { get; }
        public int EndLine { get; }

        public GitHubMultiLineSelection(int startLine, int endLine)
        {
            StartLine = startLine;
            EndLine = endLine;
        }

        public override string TransformContent(string fullContent)
        {
            var lines = fullContent.Split('\n');
            var start = Math.Min(StartLine, lines.Length);
            var end = Math.Min(EndLine, lines.Length);
            return string.Join('\n', lines[(start - 1)..end]);
        }
    }
}