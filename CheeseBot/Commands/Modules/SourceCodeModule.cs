using System.IO;
using System.Text;
using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{

    [Group("src", "source", "file")]
    public class SourceCodeModule : DiscordModuleBase
    {
        private readonly GitHubSourceBrowser _sourceBrowser;

        public SourceCodeModule(GitHubSourceBrowser sourceBrowser)
        {
            _sourceBrowser = sourceBrowser;
        }

        [Command]
        public async Task<DiscordCommandResult> SourceAsync(string path)
        {
            var (contents, filename) = await _sourceBrowser.GetFileContents(path);

            if (contents == null && filename == null)
                return Response("File could not be found");

            if (contents!.Length > LocalMessageBuilder.MAX_CONTENT_LENGTH)
            {
                var stream = new MemoryStream(Encoding.Default.GetBytes(contents));
                var msg = new LocalMessageBuilder().WithAttachments(new LocalAttachment(stream, filename)).Build();
                return Response(msg);
            }


            return Response(Markdown.CodeBlock("csharp", contents));
        }

        [Command("link")]
        public DiscordCommandResult LinkSource(string path)
            => Response($"<{_sourceBrowser.GetSourceLink(path)}>");
    }
}