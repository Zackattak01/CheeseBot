using Disqord.Bot;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    [Description("Easily converts some units")]
    public class ConversionModule : DiscordModuleBase
    {
        [Command("temperature", "temp")]
        [Description("Convert between fahrenheit and celsius")]
        public DiscordCommandResult Temperature([Remainder] string input)
        {
            string numberString = null;
            string unitString = null;
            
            for (var i = 0; i < input.Length; i++)
            {
                if (!char.IsDigit(input[i]) && !char.IsPunctuation(input[i]))
                {
                    numberString = input[..i].Trim();
                    unitString = input[i..input.Length].Trim().ToLower();
                    break;
                }
            }

            if (numberString is null)
                return Response("The input was not in the correct format!  Please use the format of `[temperature] [unit]`");

            // should never fail
            var number = double.Parse(numberString);

            return unitString switch
            {
                "c" => Response($"{numberString}C is {number * (9d / 5d) + 32}F"),
                "celsius" => Response($"{numberString}C is {number * (9d / 5d) + 32}F"),
                "f" => Response($"{numberString}F is {(number - 32) * (5d / 9d)}C"),
                "fahrenheit" => Response($"{numberString}F is {(number - 32) * (5d / 9d)}C"),
                _ => Response("Unknown unit!  Conversions to and and Fahrenheit and Celsius are supported")
            };
        }
    }
}