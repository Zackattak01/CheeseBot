using Disqord;

namespace CheeseBot.Extensions
{
    public static class MemberExtensions
    {
        public static string GetDisplayName(this IMember member)
            => member.Nick ?? member.Name;
    }
}