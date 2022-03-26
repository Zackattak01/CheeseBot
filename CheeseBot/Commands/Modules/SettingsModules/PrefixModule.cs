using System;
using System.Collections.Generic;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Group("prefix", "prefixes")]
    [Description("Guild wide prefix settings")]
    public class PrefixModule : GuildSettingsModuleBase
    {
        [Command]
        [Description("Displays the current prefixes that are recognized on this server.")]
        public DiscordCommandResult DisplayPrefixAsync()
        {
            var formattedPrefixList = new List<string>(CurrentGuildSettings.Prefixes.Count);

            foreach (var prefix in CurrentGuildSettings.Prefixes)
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
        [RequireAuthorGuildPermissions(Permission.Administrator, Group = "Permission")]
        [RequireBotOwner(Group = "Permission")]
        public  DiscordCommandResult AddPrefixAsync([Remainder] IPrefix prefix)
        {
            if (CurrentGuildSettings.Prefixes.Count >= DefaultGuildSettingsProvider.MaxNumberOfPrefixes)
                return Response($"Your server has reached the max number of prefixes ({DefaultGuildSettingsProvider.MaxNumberOfPrefixes})");
            else if (CurrentGuildSettings.Prefixes.Contains(prefix))
                return Response($"Prefix: {FormatPrefix(prefix)} is already enabled on this server.");
            else if (prefix is MentionPrefix mentionPrefix && mentionPrefix.UserId != Context.Bot.CurrentUser.Id)
                return Response("You cannot enable mentions for users other than myself as a prefix.");

            CurrentGuildSettings.Prefixes.Add(prefix);
            
            return Response($"Ok, the prefix {FormatPrefix(prefix)} will now be recognized on this server.");
        }

        [Command("remove")]
        [Description("Removes the specified prefix.")]
        [RequireAuthorGuildPermissions(Permission.Administrator, Group = "Permission")]
        [RequireBotOwner(Group = "Permission")]
        public DiscordCommandResult RemovePrefixAsync([Remainder] IPrefix prefix)
        {
            if (!CurrentGuildSettings.Prefixes.Contains(prefix))
                return Response($"The prefix {FormatPrefix(prefix)} is not enabled on this server.");
            else if (CurrentGuildSettings.Prefixes.Count == 1)
                return Response("You cannot remove the last enabled prefix on this server.");

            CurrentGuildSettings.Prefixes.Remove(prefix);

            return Response($"Ok, the prefix {FormatPrefix(prefix)} will no longer be recognized on this server.");
        }

        private string FormatPrefix(IPrefix prefix)
        {
            return prefix switch
            {
                StringPrefix => Markdown.Code(prefix),
                MentionPrefix => prefix.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(prefix), prefix, null)
            };
        }
        
    }
}