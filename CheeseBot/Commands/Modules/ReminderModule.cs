using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseBot.Disqord;
using CheeseBot.EfCore;
using CheeseBot.EfCore.Entities;
using CheeseBot.Services;
using Disqord;
using Disqord.Bot;
using Microsoft.EntityFrameworkCore;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Group("reminder", "remind", "reminders", "remindme")]
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
        public async Task<DiscordCommandResult> ReminderAsync([Remainder] Reminder reminder)
        {
            if (reminder.ExecutionTime <= DateTime.Now)
                return Response("Reminder time cannot be in the past or now.");
            
            await _reminderService.AddReminderAsync(reminder);
            return Response($"Ok, I will remind you to \"{reminder.Value}\" {reminder.GetTimeString()} (Id:{reminder.Id})");
        }

        [Command("list", "")]
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
                    var eb = new LocalEmbedBuilder().WithColor(Global.DefaultEmbedColor);

                    foreach (var reminder in reminders)
                        eb.AddField($"Reminder {reminder.Id}" , reminder.ToString());
                    
                    var content = $"You have {reminders.Count} {(reminders.Count == 1 ? "reminder" : "reminders")}";
                    return Response(content, eb);
                }
                default:
                {
                    var builders = new List<LocalEmbedFieldBuilder>(reminders.Count);

                    foreach (var reminder in reminders)
                        builders.Add(new LocalEmbedFieldBuilder().WithName($"Reminder {reminder.Id}").WithValue(reminder.ToString()));
            
                    var config = FieldBasedPageProviderConfiguration.Default.WithContent($"You have {reminders.Count} reminders");
                    return Pages(new FieldBasedPageProvider(builders, config));
                }
            }
        }
        
        [Command("remove", "cancel")]
        public async Task<DiscordCommandResult> RemoveAsync(int id)
        {
            var reminder = await _dbContext.Reminders.FindAsync(id);

            if (reminder is null)
                return Response($"A reminder with id: \"{id}\" does not exist.");
            else if (reminder.UserId != Context.Author.Id)
                return Response("You cannot remove other peoples reminders.");

            await _reminderService.RemoveReminderAsync(reminder);
            return Response($"Ok, I will no longer remind you to \"{reminder.Value}\" {reminder.GetTimeString()}");
        }
    }
}