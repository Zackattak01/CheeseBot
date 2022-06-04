namespace CheeseBot.GitHub.Entities
{
    public class GitHubSource
    {
        public string Path { get; }

        public string Filename { get; }

        public string Url { get; }

        public GitHubSource(string path, string filename, string url)
        {
            Path = path;
            Filename = filename;
            Url = url;
        }
    }
}