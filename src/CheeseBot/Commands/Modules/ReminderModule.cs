using System.Text;

namespace CheeseBot.Commands.Modules
{
    [Group("reminder", "remind", "reminders", "remindme", "r")]
    [Description("Commands for managing reminders")]
    public class ReminderModule : DiscordGuildModuleBase
    {
        private readonly ReminderService _reminderService;
        private readonly CheeseBotDbContext _dbContext;
        
        public ReminderModule(ReminderService reminderService, CheeseBotDbContext dbContext)
        {
            _reminderService = reminderService;
            _dbContext = dbContext;
        }

        [Command("", "create")]
        [Description("Creates a reminder because you we know you forgot *something* :rolling_eyes:")]
        public async Task<DiscordCommandResult> ReminderAsync([Remainder] Reminder reminder)
        {
            if (reminder.ExecutionTime <= DateTime.Now)
                return Response("Reminder cannot be set in the past.");
            
            await _reminderService.AddReminderAsync(reminder);
            
            return Response($"Ok, I will remind you {FormatReminderString(reminder)} (Id:{reminder.Id})");
        }

        [Command("list", "")]
        [Description("Lists your reminders.  I might even paginate them.")]
        public async Task<DiscordCommandResult> ListAsync()
        {
            var reminders = await _dbContext.Reminders.AsNoTracking()
                .Where(x => x.UserId == Context.Author.Id)
                .OrderBy(x => x.ExecutionTime)
                .ToListAsync();

            switch (reminders.Count)
            {
                case 0:
                    return Response("You have no reminders");
                case <= 5:
                {
                    var e = new LocalEmbed().WithDefaultColor();

                    foreach (var reminder in reminders)
                        e.AddField($"Reminder {reminder.Id}" , reminder.ToString());
                    
                    var content = $"You have {reminders.Count} {(reminders.Count == 1 ? "reminder" : "reminders")}";
                    return Response(content, e);
                }
                default:
                {
                    var builders = new List<LocalEmbedField>(reminders.Count);

                    foreach (var reminder in reminders)
                        builders.Add(new LocalEmbedField().WithName($"Reminder {reminder.Id}").WithValue(reminder.ToString()));
            
                    var config = FieldBasedPageProviderConfiguration.Default.WithContent($"You have {reminders.Count} reminders");
                    return Pages(new FieldBasedPageProvider(builders, config));
                }
            }
        }
        
        [Command("remove", "cancel")]
        [Description("Removes your reminder")]
        public async Task<DiscordCommandResult> RemoveAsync(int id)
        {
            var reminder = await _dbContext.Reminders.FindAsync(id);

            if (reminder is null)
                return Response($"A reminder with id: {id} does not exist.");
            else if (reminder.UserId != Context.Author.Id)
                return Response("You cannot remove other peoples reminders.");

            await _reminderService.RemoveReminderAsync(reminder);
            return Response($"Ok, I will no longer remind you {FormatReminderString(reminder)}");
        }

        private static string GetTimestampWithArticle(Reminder reminder) 
            => reminder.ExecutionTime.Date == DateTime.Today 
                ? Markdown.Timestamp(reminder.ExecutionTime, Markdown.TimestampFormat.RelativeTime) 
                : $"on {Markdown.Timestamp(reminder.ExecutionTime)}";

        private static string FormatReminderString(Reminder reminder)
        {
            var strBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(reminder.Value))
                strBuilder.Append("to ").Append(Markdown.Code(reminder.Value)).Append(' ');

            strBuilder.Append(GetTimestampWithArticle(reminder));
            return strBuilder.ToString();
        }
    }
}