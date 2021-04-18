using System;
using System.Threading.Tasks;

namespace CheeseBot.Scheduling
{

    public delegate Task UnhandledScheduledTaskExceptionHandler(ScheduledTask scheduledTask);
    public sealed class ScheduledTask : IEquatable<ScheduledTask>, IDisposable
    {
        private static int _idCounter;

        private readonly Func<Task> _task;

        private AsyncTimer _timer;
        public int Id { get; }

        public bool Recurring { get; }

        public DateTime ExecutionTime { get; }

        public event UnhandledScheduledTaskExceptionHandler UnhandledException;
        
        public ScheduledTask(Func<Task> task, DateTime executionTime, bool recurring = false)
        {
            Id = _idCounter++;
            ExecutionTime = executionTime;
            Recurring = recurring;
            _task = task;
            _timer = AsyncTimer.Create(executionTime - DateTime.Now);
            _timer.Elapsed += AsyncTimerElapsed;
            _timer.Start();
        }

        private async Task AsyncTimerElapsed(AsyncTimer timer)
        {
            try
            {
                await _task();
            }
            catch (Exception e)
            {
                var handler = UnhandledException;
                if (handler is not null)
                {
                    await handler(this);
                }
            }
            

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (Recurring && !_timer.AutoReset)
            {
                _timer.Dispose();
                _timer = AsyncTimer.Create(new TimeSpan(24, 0, 0), true); //create a timer that has the correct interval
            }
            else if(!Recurring)
            {
                _timer.Dispose();
            }
        }

        public bool Equals(ScheduledTask other)
        {
            if (other is null)
                return false;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ScheduledTask scheduledTask)
                return false;

            return Id == scheduledTask.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public void Dispose()
            => _timer.Dispose();
        
    }
}