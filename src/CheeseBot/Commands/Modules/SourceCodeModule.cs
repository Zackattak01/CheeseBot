using System.IO;
using System.Net;
using System.Text;
using CheeseBot.GitHub.Entities;
using Disqord.Extensions.Interactivity.Menus.Paged;

namespace CheeseBot.Commands.Modules
{

    [Group("source", "src", "file")]
    [Description("Commands for browsing Cheese Bot's source code")]
    public class SourceCodeModule : DiscordModuleBase
    {
        private readonly GitHubSourceBrowser _sourceBrowser;

        public SourceCodeModule(GitHubSourceBrowser sourceBrowser)
        {
            _sourceBrowser = sourceBrowser;
        }

        [Command]
        [Description("Returns requested source code")]
        public async Task<DiscordCommandResult> SourceAsync(string path = null)
        {
            GitHubSource gitHubSource;
            try
            {
                gitHubSource = await _sourceBrowser.GetPathAsync(path);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return Response("Requested file does not exist!");
            }
            
            switch (gitHubSource)
            {
                case null:
                    return Response("Something went wrong while fetching source.");
                case GitHubSourceFile sourceFile when string.IsNullOrWhiteSpace(sourceFile.Content):
                    return Response("The selected text is only whitespace.");
                case GitHubSourceFile sourceFile when sourceFile.Content.Length > LocalMessageBase.MaxContentLength:
                {
                    var stream = new MemoryStream(Encoding.Default.GetBytes(sourceFile.Content));
                    var msg = new LocalMessage().WithAttachments(new LocalAttachment(stream, gitHubSource.Filename));
                    return Response(msg);
                }
                case GitHubSourceFile sourceFile:
                    return Response(Markdown.CodeBlock(sourceFile.Filename.Split('.').Last(), sourceFile.Content));
                case GitHubSourceDirectory sourceDirectory:
                {
                    var view = new SelectionPagedView(new SelectionPageProvider(Array.Empty<SelectionPage>()));
                    var pageProvider = new SelectionPageProvider(GetSourceCodePages(sourceDirectory, view));
                    view.SetPageProvider(pageProvider);
                    
                    return View(view);
                }
                default:
                    throw new ArgumentException($"The type of {nameof(gitHubSource)} was not expected!", nameof(gitHubSource));
            }
        }

        [Command("link")]
        [Description("Returns a link to the requested source code")]
        public DiscordCommandResult LinkSource(string path)
            => Response($"<{GitHubSourceBrowser.GetSourceLink(path)}>");

        [Command("clear")]
        [RequireBotOwner]
        [Description("Clears the source code cache")]
        public DiscordCommandResult ClearCache()
        {
            _sourceBrowser.ClearContentCache();
            return Response("Content cache cleared!");
        }

        private IEnumerable<SelectionPage> GetSourceCodePages(GitHubSourceDirectory sourceDirectory, SelectionPagedView view)
        {
            var pages = new List<SelectionPage>(sourceDirectory.Files.Count);

            var fileListEmbed = new LocalEmbed()
                .WithDefaultColor()
                .WithTitle($"Directory: {sourceDirectory.Filename}")
                .WithDescription("**Files**\n" + string.Join("\n", sourceDirectory.Files.Select(x => Markdown.Link(x.Filename, x.Url))));
            
            var fileListPage = new Page().AddEmbed(fileListEmbed);
            pages.Add(new SelectionPage(fileListPage, "./"));

            async Task<Page> ParentFolderListPageFunc()
            {
                sourceDirectory = (GitHubSourceDirectory)await _sourceBrowser.GetPathAsync(sourceDirectory.Path + "/..");
                view.SetPageProvider(new SelectionPageProvider(GetSourceCodePages(sourceDirectory, view)));
                return await view.PageProvider.GetPageAsync(view);
            }
            pages.Add(new SelectionPage(ParentFolderListPageFunc, "../"));
            
            foreach (var file in sourceDirectory.Files)
            {
                async Task<Page> PageFunction()
                {
                    var page = new Page();
                    var fetchedFile = await _sourceBrowser.GetPathAsync(file.Path);
                    
                    switch (fetchedFile)
                    {
                        case GitHubSourceFile sourceFile:
                        {
                            if (sourceFile.Content.Length > LocalMessageBase.MaxContentLength) 
                                return page.WithContent($"File too large to display. View online: {Markdown.Link(sourceFile.Filename, sourceFile.Url)}");

                            page.WithContent(Markdown.CodeBlock("csharp", sourceFile.Content));
                            break;
                        }
                        case GitHubSourceDirectory sourceDirectory:
                        {
                            view.SetPageProvider(new SelectionPageProvider(GetSourceCodePages(sourceDirectory, view)));
                            page = await view.PageProvider.GetPageAsync(view);
                            break;
                        }
                    }

                    return page;
                }

                var page = new SelectionPage(PageFunction, file.Filename);
                pages.Add(page);
            }

            return pages;
        }
    }
}