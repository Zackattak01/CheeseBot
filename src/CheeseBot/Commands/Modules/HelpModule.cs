using Disqord.Extensions.Interactivity.Menus.Paged;

namespace CheeseBot.Commands.Modules
{
    [Description("Help and discovery tools")]
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
                    .WithDescription(module.Description)
                    .WithTitle(name);
                
                foreach (var command in module.Commands)
                    embed.AddField(command.GetHelpString(), command.Description ?? "No description");

                var page = new Page().AddEmbed(embed);
                selectionPages.Add(new SelectionPage(
                    page,
                    name,
                    module.Description?.HumanTruncateAt(LocalSelectionComponentOption.MaxDescriptionLength) ?? "No description"));
            }
            
            return View(new SelectionPagedView(new SelectionPageProvider(selectionPages)));
        }
        
        [Command("help")]
        public DiscordCommandResult Help([Remainder] string query)
        {
            var matches = _commandService.FindCommands(query);

            if (matches.Count == 0)
                return Response("No matches were found.  Please try refining your search query.");

            var selectionPages = new List<SelectionPage>
            {
                new(GetSyntaxExplanationPage(), "Help")
            };
            
            foreach (var command in matches.Select(x => x.Command))
            {
                var embed = new LocalEmbed()
                    .WithDefaultColor()
                    .WithTitle(command.GetHelpString())
                    .AddField("Description", command.Description ?? "No description");

                var displayableAliases = new List<string>();
                
                foreach (var alias in command.FullAliases)
                {
                    var trimmedAlias = alias.Trim();
                    if (trimmedAlias == command.Name || trimmedAlias == string.Empty)
                        continue;
                    
                    displayableAliases.Add($"• {command.GetHelpString(trimmedAlias)}");
                }
                
                if (displayableAliases.Count > 0)
                    embed.WithDescription($"{Markdown.Bold("Aliases")}\n{string.Join('\n', displayableAliases)}");
                
                var page = new Page().AddEmbed(embed);
                selectionPages.Add(new SelectionPage(
                    page,
                    command.Name,
                    command.Description?.HumanTruncateAt(LocalSelectionComponentOption.MaxDescriptionLength) ?? "No description"));
            }

            return View(new SelectionPagedView(new SelectionPageProvider(selectionPages)));
        }

        private static Page GetSyntaxExplanationPage()
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