namespace CheeseBot.GitHub.Entities
{
    public class GitHubSourceFile : GitHubSource
    {
        public virtual string Content { get; }
        
        public GitHubSourceFile(string path, string filename, string content)
            : base(path, filename)
        {
            Content = content;
        }
    }
}