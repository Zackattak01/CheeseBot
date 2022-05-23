namespace CheeseBot.GitHub.Entities
{
    public class GitHubSourceDirectory : GitHubSource
    {
        public IReadOnlyList<GitHubSource> Files { get; }

        public GitHubSourceDirectory(string path, string filename, IReadOnlyList<GitHubSource> files) : base(path, filename)
        {
            Files = files;
        }
    }
}