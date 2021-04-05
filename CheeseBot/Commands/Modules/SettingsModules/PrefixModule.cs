using System.Collections.Generic;
using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Group("prefix", "prefixes")]
    [RequireAuthorGuildPermissions(Permission.Administrator, Group = "Permission")]
    [RequireBotOwner(Group = "Permission")]
    public class PrefixModule : DiscordGuildModuleBase
    {
        private readonly GuildSettingsService _guildSettingsService;
        
        public PrefixModule(GuildSettingsService guildSettingsService)
        {
            _guildSettingsService = guildSettingsService;
        }

        [Command]
        public async Task DisplayPrefixAsync()
        {
            var prefixes = await _guildSettingsService.GetGuildPrefixesAsync(Context.GuildId);

            var formattedPrefixList = new List<string>(prefixes.Count);

            foreach (var prefix in prefixes)
            {
                switch (prefix)
                {
                    case MentionPrefix:
                        formattedPrefixList.Insert(0, prefix.ToString());
                        break;
                    case StringPrefix:
                        formattedPrefixList.Add(Markdown.Code(prefix.ToString()));
                        break;
                }
            }
            
            var responseString = "My prefixes for this guild are: " + string.Join(", ", formattedPrefixList);
            
            await Response(responseString);
        }

        [Command("add")]
        public async Task<DiscordCommandResult> AddPrefixAsync([Remainder] IPrefix prefix)
        {
            var currentPrefixes = await _guildSettingsService.GetGuildPrefixesAsync(Context.GuildId);

            if (currentPrefixes.Count >= DefaultGuildSettingsProvider.MaxNumberOfPrefixes)
                return Response(
                    $"Your server has reached the max number of prefixes ({DefaultGuildSettingsProvider.MaxNumberOfPrefixes})");
            else if (currentPrefixes.Contains(prefix))
                return Response($"Prefix: \"{prefix}\" is already enabled on this server.");
            else if (prefix is MentionPrefix mentionPrefix && mentionPrefix.UserId != Context.Bot.CurrentUser.Id)
                return Response("You cannot enable mentions for users other than myself as a prefix.");

            await _guildSettingsService.AddPrefixAsync(Context.GuildId, prefix);
            
            return Response($"Ok, the prefix \"{prefix}\" will now be recognized on this server.");
        }

        [Command("remove")]
        public async Task<DiscordCommandResult> RemovePrefixAsync([Remainder] IPrefix prefix)
        {
            var currentPrefixes = await _guildSettingsService.GetGuildPrefixesAsync(Context.GuildId);

            

            if (!currentPrefixes.Contains(prefix))
                return Response($"The prefix \"{prefix}\" is not enabled on this server.");
            else if (currentPrefixes.Count == 1)
                return Response("You cannot remove the last enabled prefix on this server.");

            await _guildSettingsService.RemovePrefixAsync(Context.GuildId, prefix);

            return Response($"Ok, the prefix \"{prefix}\" will no longer be recognized on this server.");
        }
    }
}