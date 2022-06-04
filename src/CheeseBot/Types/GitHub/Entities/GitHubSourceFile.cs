namespace CheeseBot.GitHub.Entities
{
    public class GitHubSourceFile : GitHubSource
    {
        public virtual string Content { get; }
        
        public GitHubSourceFile(string path, string filename, string url, string content)
            : base(path, filename, url)
        {
            Content = content;
        }
    }
}