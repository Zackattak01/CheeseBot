using System;

namespace CheeseBot.GitHub
{
    public class GitHubSourceFile
    {
        public Uri Uri { get; }
        
        public string Filename { get; }
        
        public string Content { get; }
        
        public GitHubSourceFile(Uri uri, string filename, string content)
        {
            Uri = uri;
            Filename = filename;
            Content = content;
        }
    }
}