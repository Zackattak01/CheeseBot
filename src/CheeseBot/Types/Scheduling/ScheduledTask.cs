namespace CheeseBot.Scheduling
{
    public sealed class ScheduledTask : IEquatable<ScheduledTask>, IDisposable
    {
        private static int _idCounter;

        private readonly Func<ScheduledTask,Task> _task;
        private readonly ILogger _logger;

        private AsyncTimer _timer;
        public int Id { get; }

        public bool Recurring { get; }

        public DateTime ExecutionTime { get; }
        
        public event EventHandler Disposed;
        
        public ScheduledTask(Func<ScheduledTask, Task> task, DateTime executionTime, ILogger logger, bool recurring = false)
        {
            Id = _idCounter++;
            ExecutionTime = executionTime;
            Recurring = recurring;
            _task = task;
            _logger = logger;
            _timer = new AsyncTimer(executionTime - DateTime.Now);
            InitTimer();
        }

        private void InitTimer()
        {
            _timer.Elapsed += AsyncTimerElapsed;
            _timer.UnhandledException += UnhandledException;
            _timer.Start();
        }

        private async Task AsyncTimerElapsed(object sender, EventArgs eventArgs)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (Recurring && !_timer.AutoReset)
            {
                _timer.Dispose();
                _timer = new AsyncTimer(new TimeSpan(24, 0, 0), true); //create a timer that has the correct interval
                InitTimer();
            }
            else if(!Recurring) // timer is not going to fire anymore, dispose this object
            {
                Dispose();
            }
            
            // execute the scheduled task last
            await _task(this);
        }

        private void UnhandledException(AsyncTimer sender, UnhandledExceptionEventArgs args)
            => _logger.LogError(args.Exception, "A scheduled task with id {0} threw an exception", Id);
        

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