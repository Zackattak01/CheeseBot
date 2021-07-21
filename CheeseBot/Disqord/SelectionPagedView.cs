using System;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Extensions.Interactivity.Menus;
using Disqord.Extensions.Interactivity.Menus.Paged;

namespace CheeseBot.Disqord
{
    public class SelectionPagedView : PagedViewBase
    {
        public SelectionViewComponent Selection { get; }
        
        public SelectionPagedView(SelectionPageProvider pageProvider, LocalMessage templateMessage = null) : base(
            pageProvider, templateMessage)
        {
            Selection = new SelectionViewComponent(HandleSelection);
            
            for (var i = 0; i < pageProvider.Pages.Count; i++)
            {
                var page = pageProvider.Pages[i];
                var option = new LocalSelectionComponentOption(page.SelectionLabel, i.ToString())
                    .WithDescription(page.SelectionDescription)
                    .WithEmoji(page.SelectionEmoji);
                
                Selection.Options.Add(option);
            }
            
            AddComponent(Selection);
        }

        private ValueTask HandleSelection(SelectionEventArgs e)
        {
            if (e.SelectedOptions.Count > 0)
            {
                if (!int.TryParse(e.SelectedOptions[0].Value, out var value))
                    throw new InvalidOperationException("All the values of the selection's options must be page indexes");
                
                CurrentPageIndex = value;

                if (Selection.Options.FirstOrDefault(x => x.IsDefault) is { } defaultOption)
                    defaultOption.IsDefault = false;
                
                Selection.Options.FirstOrDefault(x => x.Value == e.SelectedOptions[0].Value)!.IsDefault = true;
            }

            return default;
        }
        
    }
}