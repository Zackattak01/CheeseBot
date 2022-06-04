namespace CheeseBot.GitHub.Entities
{
    public class GitHubSourceDirectory : GitHubSource
    {
        public IReadOnlyList<GitHubSource> Files { get; }

        public GitHubSourceDirectory(string path, string filename, string url, IReadOnlyList<GitHubSource> files) : base(path, filename, url)
        {
            Files = files;
        }
    }
}