using System.Reflection;
using CheeseBot.Settings;
using CheeseBot.Settings.Formatters;

namespace CheeseBot.Commands.Modules
{
    [Description("Some general server settings")]
    public class GeneralSettingsModule : GuildSettingsModuleBase
    {
        [Command("settings")]
        public DiscordCommandResult Settings()
        {
            var attributePairs = typeof(GuildSettings).GetProperties()
                .Select(property => (Property: property, Attribute: property.GetCustomAttributes().OfType<SettingsPropertyAttribute>().SingleOrDefault()))
                .Where(x => x.Attribute is not null)
                .OrderBy(x => x.Attribute.Priority);
            
            var embed = new LocalEmbed()
                .WithDefaultColor()
                .WithTitle("Guild Settings");
            
            foreach (var pair in attributePairs)
            {
                var type = pair.Attribute.GetType();
                if (type.IsGenericType)
                {
                    var genericType = type.GetGenericArguments().First();
                    var method = genericType.GetMethod(nameof(ISettingsFormatter.Format));
                    var formattedValue = method?.Invoke(null, new object[] { CurrentGuildSettings });

                    if (formattedValue is null)
                        continue;

                    embed.AddField(pair.Attribute.Title, formattedValue);
                }
                else
                {
                    embed.AddField(pair.Attribute.Title, pair.Property.GetMethod?.Invoke(CurrentGuildSettings, null));
                }
            }

            return Response(embed);
        }
        
        [Command("permit")]
        [RequireBotOwner]
        public DiscordCommandResult PermitAsync()
        {
            if (!CurrentGuildSettings.IsPermitted)
            {
                CurrentGuildSettings.IsPermitted = true;
                return Response("This guild has been permitted!");
            }
            else
                return Response("This guild is already permitted!");
            
        }

        [Command("unpermit")]
        [RequireBotOwner]
        public DiscordCommandResult UnpermitAsync()
        {
            if (CurrentGuildSettings.IsPermitted)
            {
                CurrentGuildSettings.IsPermitted = false;
                return Response("This guild has been unpermitted!");
            }
            else
                return Response("This guild is not permitted!");
        }
    }
}