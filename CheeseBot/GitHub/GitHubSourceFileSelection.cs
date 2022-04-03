namespace CheeseBot.GitHub
{
    public class GitHubSourceFileSelection : GitHubSourceFile
    {
        public override string Content { get; }

        public GitHubSourceFileSelection(Uri uri, string filename, string content, IGitHubLineSelection selection) 
            : base(uri, filename, content)
        {
            Content = selection.TransformContent(content);
        }
        
        public GitHubSourceFileSelection(GitHubSourceFile sourceFile, IGitHubLineSelection selection) :
            this (sourceFile.Uri, sourceFile.Filename, sourceFile.Content, selection)
        {
        }
    }
}