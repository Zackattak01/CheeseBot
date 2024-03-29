using System.Diagnostics;
using Disqord.Extensions.Interactivity;

namespace CheeseBot.Commands.Modules
{
    [Description("Info about the bot, server, or a user")]
    public class InfoModule : DiscordModuleBase
    {
        [Command("ping")]
        [Description("Plays a quick game of Ping Pong.")]
        public async Task PingAsync()
        {
            const string responseString = "Pong:\nREST: {0}ms\nGateway: {1}ms";

            async Task<long> GetGatewayPingAsync()
            {
                var messageReceivedArgs = await Context.Bot.WaitForMessageAsync(Context.ChannelId, e => e.Message is IUserMessage userMessage && userMessage.Nonce == Context.Message.Id.ToString());
                return (long)(DateTimeOffset.Now - messageReceivedArgs.Message.CreatedAt()).TotalMilliseconds;
            }
            
            async Task<(IUserMessage, long)> GetRestPingAsync()
            {
                var stopwatch = Stopwatch.StartNew();
                var msg = await Response(new LocalMessage().WithContent(string.Format(responseString, "*loading*", "*loading*")).WithNonce(Context.Message.Id.ToString()));
                stopwatch.Stop();
                return (msg, stopwatch.ElapsedMilliseconds);
            }
            
            var gatewayPingTask = GetGatewayPingAsync();
            var (msg, restPing) = await GetRestPingAsync();
            var gatewayPing = await gatewayPingTask;
            
            await msg.ModifyAsync(x => x.Content = string.Format(responseString, restPing, gatewayPing));
        }

        [Command("info")]
        [Description("Provides info about the bot.")]
        public async Task<DiscordCommandResult> InfoAsync()
        {
            var authorId = Context.Bot.OwnerIds[0];
            
            using var process = Process.GetCurrentProcess();
            var uptimeString = (DateTime.Now - process.StartTime).Humanize();

            string authorString = null;

            if (Context.GuildId is not null)
            {
                var guildId = Context.GuildId.Value;
                authorString = Context.Bot.GetMember(guildId, authorId)?.Mention ??
                               (await Context.Bot.FetchMemberAsync(guildId, authorId))?.Mention;
            }

            authorString ??= Context.Bot.GetUser(authorId)?.ToString() ??
                             (await Context.Bot.FetchUserAsync(authorId))?.ToString();
            
            var embedBuilder = new LocalEmbed()
                .WithDefaultColor()
                .WithTitle(Context.Bot.CurrentUser.Name)
                .AddInlineField("Author", authorString)
                .AddInlineField("Uptime", uptimeString)
                .AddInlineBlankField()
                .AddInlineField("Source Code", Markdown.Link("GitHub", Global.CheeseBotRepo))
                .AddInlineField("Library", Markdown.Link("Disqord " + Library.Version, Library.RepositoryUrl))
                .AddInlineBlankField();
            return Response(embedBuilder);
        }

        [Command("user", "ui")]
        [Description("Gets info about the specified user")]
        public DiscordCommandResult UserInfo([Remainder] IUser user = null)
            => Response((user ?? Context.Author).CreateInfoEmbed());
        
        [Command("user", "ui")]
        [Priority(1)]
        [Description("Gets info about the specified user")]
        public DiscordCommandResult UserInfo([Remainder] IMember member)
            => Response(member.CreateInfoEmbed());

        [Command("server", "si")]
        [Description("Gets info about the current server")]
        [RequireGuild]
        public DiscordCommandResult ServerInfo()
            => Response(((DiscordGuildCommandContext) Context).Guild.CreateInfoEmbed());

        [Command("avatar", "av")]
        [Description("Fetches a user's avatar")]
        public DiscordCommandResult Avatar(IUser user, int size = 2048)
        {
            if (size < 16 || size > 2048 || (size & (size - 1)) != 0)
                return Response("Invalid size!  Size must be a power of 2 and between 16 and 2048.");

            return Response(user.GetAvatarUrl(CdnAssetFormat.Automatic, size));
        }

        [Command("avatar", "av")]
        [Description("Fetches a user's avatar")]
        public DiscordCommandResult Avatar(IMember member, int size = 2048)
            => Avatar(member as IUser, size);
        
        [Command("avatar", "av")]
        [Description("Fetches a user's avatar")]
        public DiscordCommandResult Avatar(int size = 2048)
            => Avatar(Context.Author, size);
    }
}