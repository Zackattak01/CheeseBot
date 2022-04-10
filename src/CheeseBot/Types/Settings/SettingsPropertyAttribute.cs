using CheeseBot.Settings.Formatters;

namespace CheeseBot.Settings
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingsPropertyAttribute : Attribute
    {
        public string Title { get; }
        public int Priority { get; }

        public SettingsPropertyAttribute(string title, int priority)
        {
            Title = title;
            Priority = priority;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingsPropertyAttribute<TFormatter> : SettingsPropertyAttribute 
        where TFormatter : ISettingsFormatter
    {
        public SettingsPropertyAttribute(string title, int priority)
            : base (title, priority) { }
    }
}