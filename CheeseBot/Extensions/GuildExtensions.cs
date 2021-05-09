using System.Linq;
using Disqord;
using Disqord.Gateway;

namespace CheeseBot.Extensions
{
    public static class GuildExtensions
    {
        public static LocalEmbedBuilder CreateInfoEmbed(this CachedGuild guild)
        {
            var eb = new LocalEmbedBuilder()
                .WithDefaultColor()
                .WithTitle(guild.Name)
                .WithThumbnailUrl(guild.GetIconUrl());

            if (guild.Description is not null)
                eb.WithDescription(guild.Description);

            if (guild.GetMember(guild.OwnerId) is { } owner)
                eb.AddInlineField("Owner", owner.Mention);
            
            eb.AddInlineField("Member Count", guild.MemberCount);
            eb.AddInlineField("Role Count", guild.Roles.Count);
            eb.FillLineWithEmptyFields();

            var channels = guild.GetChannels();
            eb.AddInlineField("Channel Count", channels.Count);
            eb.AddInlineField("Text Channel Count", channels.Count(x => x.Value is CachedTextChannel));
            eb.AddInlineField("Voice Channel Count", channels.Count(x => x.Value is CachedVoiceChannel));

            eb.AddInlineField("Boost Level", guild.BoostTier);
            eb.AddInlineField("Emoji Count", guild.Emojis.Count);
            eb.AddInlineField("Locale", guild.PreferredLocale);

            return eb;
        }
    }
}