using System.Collections.Generic;
using System.Threading.Tasks;
using CheeseBot.Extensions;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Group("prefix", "prefixes")]
    [RequireAuthorGuildPermissions(Permission.Administrator, Group = "Permission")]
    [RequireBotOwner(Group = "Permission")]
    public class PrefixModule : GuildSettingsModule
    {
        [Command]
        [Description("Displays the current prefixes that are recognized on this server.")]
        public DiscordCommandResult DisplayPrefixAsync()
        {
            var formattedPrefixList = new List<string>(Settings.Prefixes.Count);

            foreach (var prefix in Settings.Prefixes)
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
            
            return Response(responseString);
        }

        [Command("add")]
        [Description("Adds the specified prefix.")]
        public async Task<DiscordCommandResult> AddPrefixAsync([Remainder] IPrefix prefix)
        {

            if (Settings.Prefixes.Count >= DefaultGuildSettingsProvider.MaxNumberOfPrefixes)
                return Response(
                    $"Your server has reached the max number of prefixes ({DefaultGuildSettingsProvider.MaxNumberOfPrefixes})");
            else if (Settings.Prefixes.Contains(prefix))
                return Response($"Prefix: \"{prefix}\" is already enabled on this server.");
            else if (prefix is MentionPrefix mentionPrefix && mentionPrefix.UserId != Context.Bot.CurrentUser.Id)
                return Response("You cannot enable mentions for users other than myself as a prefix.");

            await GuildSettingsService.AddPrefixAsync(Context.GuildId, prefix);
            
            return Response($"Ok, the prefix \"{prefix}\" will now be recognized on this server.");
        }

        [Command("remove")]
        [Description("Removes the specified prefix.")]
        public async Task<DiscordCommandResult> RemovePrefixAsync([Remainder] IPrefix prefix)
        {
            var currentPrefixes = await GuildSettingsService.GetGuildPrefixesAsync(Context.GuildId);

            if (!currentPrefixes.Contains(prefix))
                return Response($"The prefix \"{prefix}\" is not enabled on this server.");
            else if (currentPrefixes.Count == 1)
                return Response("You cannot remove the last enabled prefix on this server.");

            await GuildSettingsService.RemovePrefixAsync(Context.GuildId, prefix);

            return Response($"Ok, the prefix \"{prefix}\" will no longer be recognized on this server.");
        }
    }
}