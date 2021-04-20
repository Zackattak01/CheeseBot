using System;
using System.Threading.Tasks;

namespace CheeseBot.Scheduling
{
    public sealed class ScheduledTask : IEquatable<ScheduledTask>, IDisposable
    {
        private static int _idCounter;

        private readonly Func<Task> _task;

        private AsyncTimer _timer;
        public int Id { get; }

        public bool Recurring { get; }

        public DateTime ExecutionTime { get; }

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;
        public event EventHandler Disposed;
        
        public ScheduledTask(Func<Task> task, DateTime executionTime, bool recurring = false)
        {
            Id = _idCounter++;
            ExecutionTime = executionTime;
            Recurring = recurring;
            _task = task;
            _timer = new AsyncTimer(executionTime - DateTime.Now);
            _timer.Elapsed += AsyncTimerElapsed;
            _timer.Start();
        }

        private async Task AsyncTimerElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                await _task();
            }
            catch (Exception e)
            {
                var handler = UnhandledException;
                handler?.Invoke(this, new UnhandledExceptionEventArgs(e, false));
            }
            

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (Recurring && !_timer.AutoReset)
            {
                _timer.Dispose();
                _timer = new AsyncTimer(new TimeSpan(24, 0, 0), true); //create a timer that has the correct interval
            }
            else if(!Recurring) // timer is not going to fire anymore, dispose this object
            {
                Dispose();
            }
        }

        public void Cancel()
            => Dispose();

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
        {
            _timer.Dispose();
            var handler = Disposed;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}