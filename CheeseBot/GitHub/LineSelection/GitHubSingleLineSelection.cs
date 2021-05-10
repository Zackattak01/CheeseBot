using System;

namespace CheeseBot.GitHub
{
    public class GitHubSingleLineSelection : GitHubLineSelection
    {
        public int Line { get; }

        public GitHubSingleLineSelection(int line)
        {
            Line = line;
        }

        public override string TransformContent(string fullContent)
        {
            var lines = fullContent.Split('\n');
            var index = Math.Min(Line, lines.Length);
            return lines[index - 1];
        }
    }
}