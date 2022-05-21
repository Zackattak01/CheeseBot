using Disqord.Extensions.Interactivity.Menus.Paged;

namespace CheeseBot.Disqord
{
    public class SelectionPage
    {
        private Func<Task<Page>> _pageFunc;
        private Page _page;
        
        public string SelectionLabel { get; }
        
        public string SelectionDescription { get; }
        
        public LocalEmoji SelectionEmoji { get; }
        
        public SelectionPage(Page page, string selectionLabel, string selectionDescription = null, LocalEmoji selectionEmoji = null)
        {
            _page = page;
            SelectionLabel = selectionLabel;
            SelectionDescription = selectionDescription;
            SelectionEmoji = selectionEmoji;
        }
        
        public SelectionPage(Func<Task<Page>> pageFunc, string selectionLabel, string selectionDescription = null, LocalEmoji selectionEmoji = null)
        {
            _pageFunc = pageFunc;
            SelectionLabel = selectionLabel;
            SelectionDescription = selectionDescription;
            SelectionEmoji = selectionEmoji;
        }

        public async Task<Page> GetPageAsync() 
            => _page ??= await _pageFunc();
    }
}