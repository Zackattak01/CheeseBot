using Disqord.Extensions.Interactivity.Menus.Paged;

namespace CheeseBot.Disqord
{
    public class SelectionPageProvider : PageProvider
    {
        public IReadOnlyList<SelectionPage> Pages { get; }
        
        public override int PageCount { get; }

        public SelectionPageProvider(IEnumerable<SelectionPage> selectionPages)
        {
            Pages = selectionPages.ToList();
            PageCount = Pages.Count;
        }

        public override ValueTask<Page> GetPageAsync(PagedViewBase view)
            => new(Pages[view.CurrentPageIndex].Page);
    }
}