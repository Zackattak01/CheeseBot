namespace CheeseBot.Services
{
    public class SchedulingService : CheeseBotService
    {
        private readonly Dictionary<int, ScheduledTask> _scheduledTaskDict;
        
        public SchedulingService(ILogger<SchedulingService> logger) : base(logger)
        {
            _scheduledTaskDict = new Dictionary<int, ScheduledTask>();
        }

        public ScheduledTask Schedule(DateTime time, Func<ScheduledTask, Task> task)
        {
            var scheduledTask = new ScheduledTask(task, time, Logger);
            scheduledTask.Disposed += ScheduledTaskDisposed;

            _scheduledTaskDict.Add(scheduledTask.Id, scheduledTask);
            return scheduledTask;
        }

        public ScheduledTask ScheduleRecurring(DateTime time, Func<ScheduledTask, Task> task)
        {
            var scheduledTask = new ScheduledTask(task, time, Logger, true);
            scheduledTask.Disposed += ScheduledTaskDisposed;

            _scheduledTaskDict.Add(scheduledTask.Id, scheduledTask);
            return scheduledTask;
        }

        public bool CancelScheduledTask(int id)
        {
            if (TryGetScheduledTask(id, out var task))
            {
                task.Cancel();
                return true;
            }

            return false;
        }

        public bool TryGetScheduledTask(int id, out ScheduledTask task)
            => _scheduledTaskDict.TryGetValue(id, out task);
        

        private void ScheduledTaskDisposed(object sender, EventArgs e)
            => _scheduledTaskDict.Remove((sender as ScheduledTask)!.Id);
        
    }
}