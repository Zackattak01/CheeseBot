namespace CheeseBot.Services
{
    public class DefaultGuildSettingsProvider : CheeseBotService
    {
        public const int MaxNumberOfPrefixes = 5;
        
        public List<IPrefix> DefaultPrefixes { get; } 
        
        public DefaultGuildSettingsProvider(Token token, ILogger<DefaultGuildSettingsProvider> logger) 
            : base(logger)
        {
            if (token is not BotToken botToken)
                throw new Exception("Token was not expected type");
            
            DefaultPrefixes = new List<IPrefix>
            {
                new StringPrefix("?"),
                new MentionPrefix(botToken.Id)
            };
            
            Logger.LogInformation("Default settings setup.");
        }

        public GuildSettings CreateDefaultGuildSettings(Snowflake guildId)
        {
            return new(guildId, DefaultPrefixes);
        }
    }
}