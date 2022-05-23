namespace CheeseBot.GitHub.Entities
{
    public class GitHubSourceFileSelection : GitHubSourceFile
    {
        public override string Content { get; }

        public GitHubSourceFileSelection(string path, string filename, string content, IGitHubLineSelection selection) 
            : base(path, filename, content)
        {
            Content = selection.TransformContent(content);
        }
        
        public GitHubSourceFileSelection(GitHubSourceFile sourceFile, IGitHubLineSelection selection) :
            this (sourceFile.Path, sourceFile.Filename, sourceFile.Content, selection)
        {
        }
    }
}