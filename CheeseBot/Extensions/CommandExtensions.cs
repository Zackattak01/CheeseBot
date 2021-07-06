using System.Text;
using Qmmands;

namespace CheeseBot.Extensions
{
    public static class CommandExtensions
    {
        public static string GetHelpString(this Command command)
        {
            const char space = ' ';
            
            var sb = new StringBuilder();
            sb.Append(command.Name).Append(space);

            foreach (var parameter in command.Parameters)
            {
                var paramString = parameter.IsOptional ? $"{parameter.Name}..." : parameter.Name;
                paramString = parameter.IsOptional ? $"[{paramString}]" : $"<{paramString}>";

                sb.Append(paramString).Append(space);
            }

            return sb.ToString().Trim();
        }
    }
}