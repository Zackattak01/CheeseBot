namespace CheeseBot.Extensions
{
    public static class UserExtensions
    {
        public static string GetDisplayName(this IMember member)
            => member.Nick ?? member.Name;
        
        public static LocalEmbed CreateInfoEmbed(this IUser user)
        {
            var e = new LocalEmbed()
                .WithDefaultColor()
                .WithTitle(user.ToString())
                .WithThumbnailUrl(user.GetAvatarUrl())
                .AddInlineField("Joined Discord On", Markdown.Timestamp(user.CreatedAt()));
            
            if (user is IMember member)
            {
                if (member.Nick is not null)
                    e.Title += $" ({member.Nick})";
                
                if (member.JoinedAt.HasValue)
                    e.AddInlineField("Joined Server On", Markdown.Timestamp(member.JoinedAt.Value));

                if (member.BoostedAt is not null)
                    e.AddInlineField("Boosting Since", Markdown.Timestamp(member.BoostedAt.Value));

                e.FillLineWithEmptyFields();
                
                if (member.GetPresence() is { } presence)
                {
                    e.AddInlineField("Status", presence.Status.ToString());

                    if (presence.Activities.Count > 0)
                        e.AddInlineField("Activities", string.Join('\n', presence.Activities));

                    e.FillLineWithEmptyFields();
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
                
                e.WithDescription($"Roles:\n {roleStr}");


            }

            if (user.IsBot)
                e.Title += " (Bot)";
            
            return e;
        }
    }
}