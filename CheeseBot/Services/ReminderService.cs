using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using CheeseBot.Extensions;
using CheeseBot.Scheduling;
using Disqord;
using Disqord.Gateway;
using Disqord.Hosting;
using Disqord.Rest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class ReminderService : DiscordClientService
    {
        private readonly SchedulingService _scheduler;
        
        private readonly IServiceProvider _services;

        private readonly Dictionary<int, ScheduledTask> _scheduledReminderDict;

        public ReminderService(
            SchedulingService scheduler,
            IServiceProvider services,
            ILogger<ReminderService> logger,
            DiscordClientBase client)
            : base(logger, client)
        {
            _services = services;
            _scheduler = scheduler;
            _scheduledReminderDict = new Dictionary<int, ScheduledTask>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Client.WaitUntilReadyAsync(cancellationToken);
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
                    Logger.LogInformation($"Sending missed reminder for user: {reminder.UserId}");
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
            var channel = Client.GetChannel(reminder.GuildId, reminder.ChannelId);

            // Channel where reminder was created no longer exists and hence the reminder no longer exists
            if (channel is not null)
            {
                var user = Client.GetUser(reminder.UserId) ?? await Client.FetchUserAsync(reminder.UserId);
                var localMessage = new LocalMessage()
                    .WithContent(Mention.User(user))
                    .AddEmbed(reminder.GetEmbed());

                await Client.SendMessageAsync(reminder.ChannelId, localMessage);
            }
            else
                Logger.LogInformation($"Reminder: {reminder.Id}'s channel no longer exists.  Deleting reminder without notifying user.");

            await RemoveReminderAsync(reminder);
        }

        private void ScheduleReminder(Reminder reminder)
        {
            var scheduledTask = _scheduler.Schedule(reminder.ExecutionTime, (_) => SendReminderAsync(reminder));
            _scheduledReminderDict.Add(reminder.Id, scheduledTask);
        }

        private void UnscheduleReminder(Reminder reminder)
        {
            if(_scheduledReminderDict.Remove(reminder.Id, out var scheduledTask))
                scheduledTask.Cancel();
        }
    }
}