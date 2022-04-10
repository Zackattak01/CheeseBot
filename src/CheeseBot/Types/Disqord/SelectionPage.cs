using Disqord.Extensions.Interactivity.Menus.Paged;

namespace CheeseBot.Disqord
{
    public class SelectionPage
    {
        public Page Page { get; }
        
        public string SelectionLabel { get; }
        
        public string SelectionDescription { get; }
        
        public LocalEmoji SelectionEmoji { get; }
        
        public SelectionPage(Page page, string selectionLabel, string selectionDescription = null, LocalEmoji selectionEmoji = null)
        {
            Page = page;
            SelectionLabel = selectionLabel;
            SelectionDescription = selectionDescription;
            SelectionEmoji = selectionEmoji;
        }
    }
}