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
            
            if (reminders.Count == 0)
                return Response("You have no reminders");
            
            var builders = new List<LocalEmbedFieldBuilder>(reminders.Count);

            foreach (var reminder in reminders)
                builders.Add(new LocalEmbedFieldBuilder().WithName($"Reminder {reminder.Id}").WithValue(reminder.ToString()));
            

            return Pages(new FieldBasedPageProvider(builders, 5));

        }
    }
}