using System.Text;

namespace CheeseBot.Extensions
{
    public static class CommandExtensions
    {
        public static string GetHelpString(this Command command, string forcedAlias = null)
        {
            const char space = ' ';
            
            var sb = new StringBuilder();
            sb.Append(forcedAlias ?? command.Name).Append(space);

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