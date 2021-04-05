using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Disqord.Utilities;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class ModModule : DiscordGuildModuleBase
    {
        private readonly GuildSettingsService _guildSettingsService;
        
        public ModModule(GuildSettingsService guildSettingsService)
        {
            _guildSettingsService = guildSettingsService;
        }
        
        [Command("clean")]
        [Description("Clean up after myself.")]
        [RequireAuthorChannelPermissions(Permission.ManageMessages)]
        [RequireBotChannelPermissions(Permission.ManageMessages)]
        public async Task CleanAsync([Minimum(1)] int numberToSearch = 100)
        {
            var messages = await Context.Bot.FetchMessagesAsync(Context.ChannelId, numberToSearch);
            var idsToDelete = new HashSet<Snowflake>();

            var prefixes = await _guildSettingsService.GetGuildPrefixesAsync(Context.GuildId);
            
            foreach (var message in messages)
            {
                if(message is ISystemMessage)
                    continue;

                if (message.Author.Id == Context.CurrentMember.Id)
                {
                    idsToDelete.Add(message.Id);
                }
                else
                {
                    foreach (var prefix in prefixes)
                    {
                        switch (prefix)
                        {
                            //not the best;  wish i could use prefix.TryFind, but it requires a IGatewayUserMessage which we dont have
                            case StringPrefix when message.Content.StartsWith(prefix.ToString()!):
                            case MentionPrefix when Mention.TryParseUser(message.Content.Split(' ').First(), out var result) && result == Context.CurrentMember.Id:
                                idsToDelete.Add(message.Id);
                                break;
                        }
                    }
                }
            }

            await Context.Bot.DeleteMessagesAsync(Context.ChannelId, idsToDelete);
        }

        [Command("nuke")]
        [Description("Nuke the last specified amount of message in a channel.")]
        [RequireAuthorChannelPermissions(Permission.ManageMessages)]
        [RequireBotChannelPermissions(Permission.ManageMessages)]
        public async Task NukeAsync([Minimum(1)] int numberToNuke = 100)
        {
            var messages = await Context.Bot.FetchMessagesAsync(Context.ChannelId, numberToNuke);
            await Context.Bot.DeleteMessagesAsync(Context.ChannelId, messages.Select(x => x.Id));
        }
    }
}