using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public sealed class InfoModule : DiscordModuleBase
    {
        private readonly UptimeService _uptimeService;
        
        
        public InfoModule(UptimeService uptimeService)
        {
            _uptimeService = uptimeService;
        }


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

            var authorString = Context.Bot.GatewayClient.GetUser(authorId).ToString();;

            //TODO: CHECK UP
            //at this time of writing this Guild.Members is null
            //i believe that its not implemented yet
            if (Context.GuildId is not null)
            {
                var member = Context.Bot.GetMember(Context.GuildId.Value, authorId);
                if (member is not null)
                    authorString = member.ToString();
            }

            var embedBuilder = new LocalEmbedBuilder()
                .WithColor(Global.DefaultEmbedColor)
                .WithTitle(Context.Bot.CurrentUser.Name)
                .AddField("Author", authorString, true)
                .AddField("Uptime", _uptimeService.Uptime.GetHumanReadableTimeFormat(), true)
                .AddBlankField()
                .AddField("Source Code", Markdown.Link("GitHub", Global.CheeseBotRepo), true)
                .AddField("Library", Markdown.Link("Disqord " + Library.Version.ToString(), Library.RepositoryUrl), true);


            return Response(embedBuilder);
        }
    }
}