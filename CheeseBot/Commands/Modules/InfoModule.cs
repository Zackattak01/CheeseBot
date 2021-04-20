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
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var msg = await Response("Pong: *loading* response time");
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = $"Pong: {stopwatch.ElapsedMilliseconds}ms response time");
        }

        [Command("info")]
        [Description("Provides info about the bot.")]
        public DiscordCommandResult InfoAsync()
        {
            var authorId = Context.Bot.OwnerIds[0];

            var authorString = Context.Bot.GetUser(authorId).ToString();

            var uptimeString = (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize();
            
            if (Context.GuildId is not null)
            {
                var member = Context.Bot.GetMember(Context.GuildId.Value, authorId);
               
                if (member is not null)
                    authorString = member.Mention;
            }

            var embedBuilder = new LocalEmbedBuilder()
                .WithColor(Global.DefaultEmbedColor)
                .WithTitle(Context.Bot.CurrentUser.Name)
                .AddField("Author", authorString, true)
                .AddField("Uptime", uptimeString, true)
                .AddBlankField(isInline:true)
                .AddField("Source Code", Markdown.Link("GitHub", Global.CheeseBotRepo), true)
                .AddField("Library", Markdown.Link("Disqord " + Library.Version, Library.RepositoryUrl), true)
                .AddBlankField(isInline:true);
            
            return Response(embedBuilder);
        }
    }
}