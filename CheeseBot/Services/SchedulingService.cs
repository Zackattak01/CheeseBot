using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheeseBot.Scheduling;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class SchedulingService : CheeseBotService
    {
        private readonly Dictionary<int, ScheduledTask> _scheduledTaskDict;
        
        public SchedulingService(ILogger<SchedulingService> logger) : base(logger)
        {
            _scheduledTaskDict = new Dictionary<int, ScheduledTask>();
        }

        public ScheduledTask Schedule(DateTime time, Func<Task> task)
        {
            var scheduledTask = new ScheduledTask(task, time);
            scheduledTask.Disposed += ScheduledTaskDisposed;
            scheduledTask.UnhandledException += ScheduledTaskUnhandledException;
            
            _scheduledTaskDict.Add(scheduledTask.Id, scheduledTask);
            return scheduledTask;
        }

        public ScheduledTask ScheduleRecurring(DateTime time, Func<Task> task)
        {
            var scheduledTask = new ScheduledTask(task, time, true);
            scheduledTask.Disposed += ScheduledTaskDisposed;
            scheduledTask.UnhandledException += ScheduledTaskUnhandledException;
            
            _scheduledTaskDict.Add(scheduledTask.Id, scheduledTask);
            return scheduledTask;
        }

        public bool TryGetScheduledTask(int id, out ScheduledTask task)
            => _scheduledTaskDict.TryGetValue(id, out task);
        
        private void ScheduledTaskUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var task = sender as ScheduledTask;
            Logger.LogError(
                $"A scheduled task with id: {task!.Id} threw an exception:\n {unhandledExceptionEventArgs.ExceptionObject}");
        }

        private void ScheduledTaskDisposed(object sender, EventArgs e)
            => _scheduledTaskDict.Remove((sender as ScheduledTask)!.Id);
        
    }
}