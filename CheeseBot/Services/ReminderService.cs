using System.Threading;

namespace CheeseBot.Services
{
    public class ReminderService : DiscordBotService
    {
        private readonly SchedulingService _scheduler;
        
        private readonly IServiceProvider _services;

        private readonly Dictionary<int, ScheduledTask> _scheduledReminderDict;

        public ReminderService(
            SchedulingService scheduler,
            IServiceProvider services,
            ILogger<ReminderService> logger,
            DiscordBotBase bot)
            : base(logger, bot)
        {
            _services = services;
            _scheduler = scheduler;
            _scheduledReminderDict = new Dictionary<int, ScheduledTask>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Bot.WaitUntilReadyAsync(cancellationToken);
            await RescheduleExistingReminders();
        }

        public async Task AddReminderAsync(Reminder reminder)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();

            await dbContext.Reminders.AddAsync(reminder);
            await dbContext.SaveChangesAsync();

            // prevents very small time values from showing errors to the user
            try
            {
                ScheduleReminder(reminder);
            }
            catch (ArgumentException)
            {
                await SendReminderAsync(reminder);
            }
            
        }
        
        public async Task RemoveReminderAsync(Reminder reminder)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();

            UnscheduleReminder(reminder);
            dbContext.Reminders.Remove(reminder);
            await dbContext.SaveChangesAsync();
        }

        private async Task RescheduleExistingReminders()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CheeseBotDbContext>();

            
            Logger.LogInformation("Scheduling preexisting reminders...");
            foreach (var reminder in await dbContext.Reminders.ToListAsync())
            {
                if (reminder.ExecutionTime <= DateTime.Now)
                {
                    Logger.LogInformation("Sending missed reminder for user: {0}", reminder.UserId);
                    await SendReminderAsync(reminder);
                }
                else
                {
                    ScheduleReminder(reminder);
                }
            }
            Logger.LogInformation("Done scheduling preexisting reminders.");
        }

        private async Task SendReminderAsync(Reminder reminder)
        {
            var channel = Bot.GetChannel(reminder.GuildId, reminder.ChannelId);
            
            var localMessage = new LocalMessage()
                .WithContent(Mention.User(reminder.UserId))
                .AddEmbed(reminder.CreateEmbed());

            // Channel where reminder was created no longer exists and hence the reminder no longer exists
            if (channel is not null)
                await Bot.SendMessageAsync(reminder.ChannelId, localMessage);
            else
            {
                try
                {
                    localMessage.Embeds.First().WithFooter("Your are receiving a DM because the channel this reminder was scheduled in no longer exists");
                    var dmChannel = await Bot.CreateDirectChannelAsync(reminder.UserId);
                    await dmChannel.SendMessageAsync(localMessage);
                }
                catch (RestApiException e) when (e.ErrorModel.Code.GetValueOrDefault() == RestApiErrorCode.CannotSendMessagesToThisUser)
                {
                    Logger.LogInformation("Could not notify user {0} about a reminder.  Deleting reminder without notifying user.", reminder.UserId);
                }
            }

            await RemoveReminderAsync(reminder);
        }

        private void ScheduleReminder(Reminder reminder)
        {
            var scheduledTask = _scheduler.Schedule(reminder.ExecutionTime, _ => SendReminderAsync(reminder));
            _scheduledReminderDict.Add(reminder.Id, scheduledTask);
        }

        private void UnscheduleReminder(Reminder reminder)
        {
            if(_scheduledReminderDict.Remove(reminder.Id, out var scheduledTask))
                scheduledTask.Cancel();
        }
    }
}