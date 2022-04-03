using System.Timers;

namespace CheeseBot.Scheduling
{
    public sealed class AsyncTimer : IDisposable
    {
        private readonly Timer _timer;

        private double _remainingInterval;

        public event AsynchronousEventHandler<AsyncTimer, EventArgs> Elapsed;
        public event Action<AsyncTimer, UnhandledExceptionEventArgs> UnhandledException;
        
        public bool AutoReset { get; }
        
        public double Interval { get; }

        public AsyncTimer(double interval, bool autoReset = false)
        {
            if (interval <= 0)
                throw new ArgumentException($"{nameof(interval)} cannot be less than or equal to zero");
            
            Interval = interval;
            AutoReset = autoReset;
            
            _remainingInterval = interval;
            _timer = new Timer();
            
            InitTimerHandlers();
        }

        public AsyncTimer(TimeSpan interval, bool autoReset = false)
            : this(interval.TotalMilliseconds, autoReset)
        {
        }

        private void InitTimerHandlers()
        {
            // _timer.Interval cannot be greater than int.MaxValue despite being a double
            if (Interval > int.MaxValue)
            {
                _timer.Interval = int.MaxValue;
                _timer.AutoReset = true;
                _timer.Elapsed += IntMaxValueHandler;
            }
            else
            {
                _timer.Interval = Interval;
                _timer.AutoReset = AutoReset;
                _timer.Elapsed += TimerElapsed;
            }
        }
        
        private void IntMaxValueHandler(object sender, ElapsedEventArgs e)
        {
            _remainingInterval -= int.MaxValue;

            // interval is still greater than int.MaxValue so no action is needed
            if (_remainingInterval > int.MaxValue)
                return;
            
            // we can assume the next time the timer elapses no further correction is needed
            _timer.AutoReset = AutoReset;
            _timer.Interval = _remainingInterval;
            _timer.Elapsed -= IntMaxValueHandler;
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _ = TimerElapsedAsync(sender, e);
            HandleAutoReset();
        }

        private async Task TimerElapsedAsync(object sender, ElapsedEventArgs e)
        {
            var handler = Elapsed;
            if (handler is not null)
            {
                try
                {
                    await handler(this, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    var exceptionHandler = UnhandledException;
                    exceptionHandler?.Invoke(this, new UnhandledExceptionEventArgs(exception));
                }
            }
        }

        private void HandleAutoReset()
        {
            if (!AutoReset)
                return;

            _timer.Elapsed -= TimerElapsed;
            InitTimerHandlers();
        }

        public void Start()
            => _timer.Start();
        
        public void Stop()
            => _timer.Stop();

        public void Dispose()
            => _timer.Dispose();
    }
}