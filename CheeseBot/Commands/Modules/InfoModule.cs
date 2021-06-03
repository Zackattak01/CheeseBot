using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CheeseBot.Extensions;
using CheeseBot.Scheduling;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class InfoModule : DiscordModuleBase
    {
        private readonly SchedulingService _schedulingService;
        public InfoModule(SchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
        }

        [Command("ping")]
        [Description("Plays a quick game of Ping Pong.")]
        public async Task PingAsync()
        {
            const string responseString = "Pong:\nREST: {0}ms\nGateway: {1}ms";
            var gatewayLatency = DateTimeOffset.Now - Context.Message.CreatedAt();

            var stopwatch = Stopwatch.StartNew();
            var msg = await Response(string.Format(responseString, "*loading*", "*loading*"));
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = string.Format(responseString, stopwatch.ElapsedMilliseconds, (int)gatewayLatency.TotalMilliseconds));
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

        [Command("av", "avatar")]
        public DiscordCommandResult Avatar(IUser user, int size = 2048)
        {
            if (size < 16 || size > 2048 || (size & (size - 1)) != 0)
                return Response("Invalid size!  Size must be a power of 2 and between 16 and 2048.");

            return Response(user.GetAvatarUrl(size: size));
        }

        [Command("av", "avatar")]
        public DiscordCommandResult Avatar(IMember member, int size = 2048)
            => Avatar(member as IUser, size);
        
        [Command("av", "avatar")]
        public DiscordCommandResult Avatar(int size = 2048)
            => Avatar(Context.Author, size);

        [Command("timer")]
        public void Timer()
        {
            // var timer = new AsyncTimer(5000, true);
            // timer.Elapsed += (sender, args) =>
            // {
            //     Console.WriteLine("throwing");
            //     throw new Exception("Testing exception");
            // };
            //
            // timer.UnhandledException += (_, args) => Console.WriteLine("Unhandled Exception!\n" + args.Exception);
            // timer.Start();

            _schedulingService.ScheduleRecurring(DateTime.Now.AddSeconds(5),
                task => throw new Exception("Doing exception things"));
        }
    }
}