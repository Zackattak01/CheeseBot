using System.Collections.Generic;
using System.Linq;
using Disqord;
using Disqord.Gateway;

namespace CheeseBot.Extensions
{
    public static class UserExtensions
    {
        public static string GetDisplayName(this IMember member)
            => member.Nick ?? member.Name;
        
        public static LocalEmbedBuilder CreateInfoEmbed(this IUser user)
        {
            const string dtoFormat = "M/d/yy h:mm tt zzz";

            var eb = new LocalEmbedBuilder()
                .WithDefaultColor()
                .WithTitle(user.ToString())
                .WithThumbnailUrl(user.GetAvatarUrl())
                .AddField("Joined Discord On", user.CreatedAt.ToString(dtoFormat), isInline: true);

            if (user is IMember member)
            {
                if (member.Nick is not null)
                    eb.Title += $" ({member.Nick})";

                if (member.JoinedAt.HasValue)
                    eb.AddField("Joined Server On", member.JoinedAt.Value.ToString(dtoFormat), isInline: true);

                if (member.BoostedAt is not null)
                    eb.AddField("Boosting Since", member.BoostedAt.Value.ToString(dtoFormat), isInline: true);

                eb.FillLineWithEmptyFields();
                
                if (member.GetPresence() is { } presence)
                {
                    eb.AddField("Status", presence.Status.ToString(), isInline: true);

                    if (presence.Activities.Count > 0)
                        eb.AddField("Activities", string.Join('\n', presence.Activities), isInline: true);

                    eb.FillLineWithEmptyFields();
                }
                
                
                string roleStr;
                // all roles cached
                if (member.GetRoles().Count - 1 == member.RoleIds.Count)
                {
                    var roles = member.GetRoles().OrderByDescending(x => x.Value.Position);

                    var roleStrings = new List<string>(member.RoleIds.Count);
                    foreach (var kvp in roles)
                    {
                        // cant wait till discord removes this feature
                        // checks for everyone role
                        if (kvp.Key == member.GuildId)
                            continue;
                        
                        roleStrings.Add(kvp.Value.Mention);
                    }
                    roleStr = string.Join('\n', roleStrings);
                }
                else
                    roleStr = string.Join('\n', member.RoleIds.Select(Mention.Role));
                
                eb.WithDescription($"Roles:\n {roleStr}");


            }

            if (user.IsBot)
                eb.Title += " (Bot)";
            
            return eb;
        }
    }
}