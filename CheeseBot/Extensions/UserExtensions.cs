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
                .AddInlineField("Joined Discord On", user.CreatedAt.ToString(dtoFormat));

            if (user is IMember member)
            {
                if (member.Nick is not null)
                    eb.Title += $" ({member.Nick})";
                
                if (member.JoinedAt.HasValue)
                    eb.AddInlineField("Joined Server On", member.JoinedAt.Value.ToString(dtoFormat));

                if (member.BoostedAt is not null)
                    eb.AddInlineField("Boosting Since", member.BoostedAt.Value.ToString(dtoFormat));

                eb.FillLineWithEmptyFields();
                
                if (member.GetPresence() is { } presence)
                {
                    eb.AddInlineField("Status", presence.Status.ToString());

                    if (presence.Activities.Count > 0)
                        eb.AddInlineField("Activities", string.Join('\n', presence.Activities));

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