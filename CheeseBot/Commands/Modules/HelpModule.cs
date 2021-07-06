using System.Collections.Generic;
using System.Threading.Tasks;
using CheeseBot.Extensions;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus;
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
            return View(HelpView.Create(_commandService));
        }

        private ListPageProvider CreateHelpPages()
        {
            var pages = new List<Page>();

            foreach (var module in _commandService.GetAllModules())
            {
                var embed = new LocalEmbed().WithTitle(module.Name);
                pages.Add(new Page().AddEmbed(embed));
            }

            return new ListPageProvider(pages);
        }
        
        public class HelpView : PagedViewBase
        {
            private HelpView(PageProvider provider, IList<LocalSelectionComponentOption> options)
                : base(provider)
            {
                CreateModuleSelectionComponent(options);
            }

            private void CreateModuleSelectionComponent(IList<LocalSelectionComponentOption> options)
            {
                var component = new SelectionViewComponent(HandleModuleSelection)
                {
                    Options = options
                };
                AddComponent(component);
            }

            private ValueTask HandleModuleSelection(SelectionEventArgs e)
            {
                if (e.SelectedOptions.Count > 0)
                    CurrentPageIndex = int.Parse(e.SelectedOptions[0].Value);

                return ValueTask.CompletedTask;
            }

            public static HelpView Create(CommandService commandService)
            {
                var pages = new List<Page>();
                var selectionMap = new List<LocalSelectionComponentOption>();

                var startingPage = new LocalEmbed()
                    .WithDefaultColor()
                    .WithTitle("Help")
                    .WithDescription(@"<argName> = required argument
                                       <argName...> = argument will be parsed with the remaining text
                                       [argName] = optional argument
                                       [argName...] = a combination of the last two")
                    .WithFooter("To begin viewing modules select one below!");
                
                pages.Add(new Page().AddEmbed(startingPage));
                selectionMap.Add(new LocalSelectionComponentOption("Home", "0"));
                
                var modules = commandService.GetAllModules();
                for (var i = 0; i < modules.Count; i++)
                {
                    var module = modules[i];
                    var name = module.Name.DeCamelCase();

                    var embed = new LocalEmbed()
                        .WithDefaultColor()
                        .WithTitle(name);

                    foreach (var command in module.Commands)
                    {
                        embed.AddField(command.GetHelpString(), command.Description ?? "No description");
                    }
                    
                    pages.Add(new Page().AddEmbed(embed));
                    
                    selectionMap.Add(new LocalSelectionComponentOption(name, (i + 1).ToString()));
                }
                return new HelpView(new ListPageProvider(pages), selectionMap);
            }

            
            
        }
    }
}