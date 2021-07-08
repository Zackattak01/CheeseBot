using System.Collections.Generic;
using CheeseBot.Disqord;
using CheeseBot.Extensions;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Qmmands;

namespace CheeseBot.Commands.Modules
{
    public class HelpModule : DiscordModuleBase
    {
        private readonly CommandService _commandService;

        public HelpModule(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("help")]
        [Description("Shows info about commands")]
        public DiscordCommandResult Help()
        {
            var modules = _commandService.GetAllModules();

            var selectionPages = new List<SelectionPage>
            {
                new(GetSyntaxExplanationPage(), "Help")
            };
            
            foreach (var module in modules)
            {
                var name = module.Name.DeCamelCase();
                
                var embed = new LocalEmbed()
                    .WithDefaultColor()
                    .WithTitle(name);
                
                foreach (var command in module.Commands)
                    embed.AddField(command.GetHelpString(), command.Description ?? "No description");

                var page = new Page().AddEmbed(embed);
                selectionPages.Add(new SelectionPage(page, name, module.Description));
            }
            
            return View(new SelectionPagedView(new SelectionPageProvider(selectionPages)));
        }

        private Page GetSyntaxExplanationPage()
        {
            const string description = "<argName> = required argument\n" +
                                       "<argName...> = argument will be parsed with the remaining text\n" + 
                                       "[argName] = optional argument\n" + 
                                       "[argName...] = an optional argument that will be parsed with the remaining text\n";
     
            return new Page().AddEmbed(new LocalEmbed()
                .WithDefaultColor()
                .WithTitle("Help")
                .WithDescription(description)
                .WithFooter("Select an option below to begin!"));
        }
    }
}