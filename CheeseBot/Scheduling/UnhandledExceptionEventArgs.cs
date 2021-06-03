using System;

namespace CheeseBot.Scheduling
{
    public class UnhandledExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public UnhandledExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}