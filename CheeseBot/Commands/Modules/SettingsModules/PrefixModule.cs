using System.Collections.Generic;
using System.Threading.Tasks;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Utilities;
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
        public async Task<DiscordCommandResult> AddPrefixAsync([Remainder] string prefix)
        {
            var currentPrefixes = await _guildSettingsService.GetGuildPrefixesAsync(Context.GuildId);

            IPrefix newPrefix;

            if (Mention.TryParseUser(prefix, out var result))
            {
                if (result != Context.Bot.CurrentUser.Id)
                    return Response("You cannot enable mentions for users other than myself as a prefix.");
                newPrefix = new MentionPrefix(result);
            }
            else
                newPrefix = new StringPrefix(prefix);

            if (currentPrefixes.Count >= DefaultGuildSettingsProvider.MaxNumberOfPrefixes)
                return Response(
                    $"Your server has reached the max number of prefixes ({DefaultGuildSettingsProvider.MaxNumberOfPrefixes})");
            else if (currentPrefixes.Contains(newPrefix))
                return Response($"Prefix: \"{prefix}\" is already enabled on this server.");

            await _guildSettingsService.AddPrefixAsync(Context.GuildId, newPrefix);
            
            return Response($"Ok, the prefix \"{prefix}\" will now be recognized on this server.");
        }

        [Command("remove")]
        public async Task<DiscordCommandResult> RemovePrefixAsync([Remainder] string prefix)
        {
            var currentPrefixes = await _guildSettingsService.GetGuildPrefixesAsync(Context.GuildId);

            IPrefix prefixToRemove;

            if (Mention.TryParseUser(prefix, out var result))
                prefixToRemove = new MentionPrefix(result);
            else
                prefixToRemove = new StringPrefix(prefix);

            if (!currentPrefixes.Contains(prefixToRemove))
                return Response($"The prefix \"{prefix}\" is not enabled on this server.");
            else if (currentPrefixes.Count == 1)
                return Response("You cannot remove the last enabled prefix on this server.");

            await _guildSettingsService.RemovePrefixAsync(Context.GuildId, prefixToRemove);

            return Response($"Ok, the prefix \"{prefix}\" will no longer be recognized on this server.");
        }
    }
}