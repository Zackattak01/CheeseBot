using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CheeseBot.Extensions;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class InfoModule : DiscordModuleBase
    {
        [Command("ping")]
        [Description("Plays a quick game of Ping Pong.")]
        public async Task PingAsync()
        {
            var now = DateTimeOffset.Now;
            const string responseString = "Pong:\nREST: {0}ms\nGateway: {1}ms";
            
            var stopwatch = Stopwatch.StartNew();
            var msg = await Response(string.Format(responseString, "*loading*", "*loading*"));
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = string.Format(responseString, stopwatch.ElapsedMilliseconds, (int)(now - Context.Message.CreatedAt).TotalMilliseconds));
        }

        [Command("info")]
        [Description("Provides info about the bot.")]
        public DiscordCommandResult InfoAsync()
        {
            var authorId = Context.Bot.OwnerIds[0];

            var authorString = Context.Bot.GetUser(authorId).ToString();

            using var process = Process.GetCurrentProcess();
            var uptimeString = (DateTime.Now - process.StartTime).Humanize();
            
            if (Context.GuildId is not null)
            {
                var member = Context.Bot.GetMember(Context.GuildId.Value, authorId);
               
                if (member is not null)
                    authorString = member.Mention;
            }

            var embedBuilder = new LocalEmbedBuilder()
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

        [Command("ui", "user")]
        [Description("Gets info about the specified user")]
        public DiscordCommandResult UserInfo([Remainder] IUser user = null)
            => Response((user ?? Context.Author).CreateInfoEmbed());
        
        [Command("ui", "user")]
        [Priority(1)]
        [Description("Gets info about the specified user")]
        public DiscordCommandResult UserInfo([Remainder] IMember member)
            => Response(member.CreateInfoEmbed());

        [Command("si", "server")]
        [Description("Gets info about the current server")]
        [RequireGuild]
        public DiscordCommandResult ServerInfo()
            => Response(((DiscordGuildCommandContext) Context).Guild.CreateInfoEmbed());
    }
}