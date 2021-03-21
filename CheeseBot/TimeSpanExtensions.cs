using System;
using System.Text;

namespace CheeseBot
{
    public static class TimeSpanExtensions
    {
        private const string DayFormat = "%d' day '";
        private const string DaysFormat = "%d' days '";
        private const string HourFormat = "%h' hour '";
        private const string HoursFormat = "%h' hours '";
        private const string MinuteFormat = "%m' min '";
        private const string MinutesFormat = "%m' mins '";
        private const string SecondFormat = "%s' sec '";
        private const string SecondsFormat = "%s' secs '";


        //simple formatter for timespans to avoid:  0 days 0 hours 0 minutes 30 seconds
        //instead it will produce:  30 seconds
        public static string GetHumanReadableTimeFormat(this TimeSpan ts)
        {
            //"%d' days '%h' hours '%m' mins '%s' secs'"
            var formatBuilder = new StringBuilder();

            if (ts.Days == 1)
                formatBuilder.Append(DayFormat);
            else if(ts.Days > 1)
                formatBuilder.Append(DaysFormat);

            if (ts.Hours == 1)
                formatBuilder.Append(HourFormat);
            else if(ts.Hours > 1)
                formatBuilder.Append(HoursFormat);

            if (ts.Minutes == 1)
                formatBuilder.Append(MinuteFormat);
            else if(ts.Minutes > 1)
                formatBuilder.Append(MinutesFormat);

            if (ts.Seconds == 1)
                formatBuilder.Append(SecondFormat);
            else if (ts.Seconds > 1)
                formatBuilder.Append(SecondsFormat);

            return ts.ToString(formatBuilder.ToString());
        }
    }
}