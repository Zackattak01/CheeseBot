using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        public async Task PingAsync()
        {
            var localMessage = new LocalMessageBuilder().WithContent("Pong: *loading* response time");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var msg = await Context.Bot.SendMessageAsync(Context.ChannelId, localMessage.Build());
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = $"Pong: {stopwatch.ElapsedMilliseconds}ms response time");
        }

        [Command("info")]
        public DiscordCommandResult InfoAsync()
        {
            var authorId = Context.Bot.OwnerIds[0];

            var authorString = Context.Bot.GatewayClient.GetUser(authorId).ToString();

            var uptimeString = (DateTime.Now - Process.GetCurrentProcess().StartTime).GetHumanReadableTimeFormat();
            
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