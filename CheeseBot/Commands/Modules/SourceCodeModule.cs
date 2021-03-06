using System.IO;
using System.Text;
using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

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
        public async Task<DiscordCommandResult> SourceAsync(string path)
        {
            var sourceFile = await _sourceBrowser.GetFileContentsAsync(path);

            if (sourceFile is null)
                return Response("File could not be found or something else went wrong.");

            if (string.IsNullOrWhiteSpace(sourceFile.Content))
                return Response("The selected text is only whitespace");
            
            if (sourceFile.Content.Length > LocalMessageBase.MaxContentLength)
            {
                var stream = new MemoryStream(Encoding.Default.GetBytes(sourceFile.Content));
                var msg = new LocalMessage().WithAttachments(new LocalAttachment(stream, sourceFile.Filename));
                return Response(msg);
            }


            return Response(Markdown.CodeBlock("csharp", sourceFile.Content));
        }

        [Command("link")]
        [Description("Returns a link to the requested source code")]
        public DiscordCommandResult LinkSource(string path)
            => Response($"<{_sourceBrowser.GetSourceLink(path)}>");

        [Command("clear")]
        [RequireBotOwner]
        [Description("Clears the source code cache")]
        public DiscordCommandResult ClearCache()
        {
            _sourceBrowser.ClearContentCache();
            return Response("Content cache cleared!");
        }
    }
}