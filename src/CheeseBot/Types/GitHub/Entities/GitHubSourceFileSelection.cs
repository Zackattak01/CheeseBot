namespace CheeseBot.GitHub.Entities
{
    public class GitHubSourceFileSelection : GitHubSourceFile
    {
        public override string Content { get; }

        public GitHubSourceFileSelection(string path, string filename, string url, string content, IGitHubLineSelection selection) 
            : base(path, filename, url, content)
        {
            Content = selection.TransformContent(content);
        }
        
        public GitHubSourceFileSelection(GitHubSourceFile sourceFile, IGitHubLineSelection selection) :
            this (sourceFile.Path, sourceFile.Filename, sourceFile.Url, sourceFile.Content, selection)
        {
        }
    }
}