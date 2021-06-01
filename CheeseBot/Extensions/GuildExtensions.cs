using System.Linq;
using Disqord;
using Disqord.Gateway;

namespace CheeseBot.Extensions
{
    public static class GuildExtensions
    {
        public static LocalEmbed CreateInfoEmbed(this CachedGuild guild)
        {
            var e = new LocalEmbed()
                .WithDefaultColor()
                .WithTitle(guild.Name)
                .WithThumbnailUrl(guild.GetIconUrl());

            if (guild.Description is not null)
                e.WithDescription(guild.Description);

            if (guild.GetMember(guild.OwnerId) is { } owner)
                e.AddInlineField("Owner", owner.Mention);
            
            e.AddInlineField("Member Count", guild.MemberCount);
            e.AddInlineField("Role Count", guild.Roles.Count);
            e.FillLineWithEmptyFields();

            var channels = guild.GetChannels();
            e.AddInlineField("Channel Count", channels.Count);
            e.AddInlineField("Text Channel Count", channels.Count(x => x.Value is CachedTextChannel));
            e.AddInlineField("Voice Channel Count", channels.Count(x => x.Value is CachedVoiceChannel));

            e.AddInlineField("Boost Level", guild.BoostTier);
            e.AddInlineField("Emoji Count", guild.Emojis.Count);
            e.AddInlineField("Locale", guild.PreferredLocale);

            return e;
        }
    }
}