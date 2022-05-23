namespace CheeseBot.GitHub.Entities
{
    public class GitHubSource
    {
        public string Path { get; }

        public string Filename { get; }

        public GitHubSource(string path, string filename)
        {
            Path = path;
            Filename = filename;
        }
    }
}