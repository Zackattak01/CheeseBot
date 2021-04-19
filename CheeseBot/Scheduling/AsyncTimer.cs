using System;
using System.Timers;
using Disqord.Events;

// using Timer = System.Timers.Timer;

namespace CheeseBot.Scheduling
{

    public sealed class AsyncTimer : IDisposable
    {
        private readonly Timer _timer;

        private double _remainingInterval;

        public event AsynchronousEventHandler<EventArgs> Elapsed;
        
        public bool AutoReset { get; }
        
        public double Interval { get; }

        private AsyncTimer(double interval, bool autoReset)
        {
            Interval = interval;
            AutoReset = autoReset;
            
            _remainingInterval = interval;
            _timer = new Timer();
            
            InitTimerHandlers();
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
            // apparently this avoids a rare race condition
            // its not likely I would ever produce one, considering the way I plan to use this class
            // but at least we prevent it from happening
            var handler = Elapsed;
            if (handler is not null)
            {
                _ = handler(this, new EventArgs());
            }
            
            HandleAutoReset();
        }

        private void HandleAutoReset()
        {
            if (!AutoReset)
                return;

            _timer.Elapsed -= TimerElapsed;
            InitTimerHandlers();
        }

        public static AsyncTimer Create(TimeSpan timeSpan, bool autoReset = false)
        {
            if (timeSpan <= TimeSpan.Zero)
                throw new ArgumentException($"{nameof(timeSpan)} cannot be less than or equal to TimeSpan.Zero");

            return new AsyncTimer(timeSpan.TotalMilliseconds, autoReset);
        }
        
        public static AsyncTimer Create(double interval, bool autoReset = false)
        {
            if (interval <= 0)
                throw new ArgumentException($"{nameof(interval)} cannot be less than or equal to zero");

            return new AsyncTimer(interval, autoReset);
        }

        public void Start()
            => _timer.Start();
        

        public void Stop()
            => _timer.Stop();
        
        
        public void Dispose()
            => _timer.Dispose();
        
        
    }
}