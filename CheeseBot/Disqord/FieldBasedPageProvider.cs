using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Extensions.Interactivity.Menus.Paged;

namespace CheeseBot.Disqord
{
    public class FieldBasedPageProvider : IPageProvider
    {
        private readonly List<Page> _pages;

        private readonly FieldBasedPageProviderConfiguration _configuration;

        public int PageCount { get; }

        public FieldBasedPageProvider(IEnumerable<LocalEmbedFieldBuilder> fields, FieldBasedPageProviderConfiguration configuration = null)
        {
            _configuration = configuration ?? FieldBasedPageProviderConfiguration.Default;
            _pages = new List<Page>();

            var fieldArray = fields as LocalEmbedFieldBuilder[] ?? fields.ToArray();
            var totalFields = fieldArray.Length;
            // this is ugly, but ambiguous calls and math.ceiling returning a double forced my hand
            PageCount = (int)Math.Ceiling((decimal)totalFields / _configuration.FieldsPerPage);
            
            CreatePages(fieldArray, _configuration.FieldsPerPage);
        }

        private void CreatePages(LocalEmbedFieldBuilder[] fields, int fieldsPerPage)
        {
            var loopLimit = PageCount * fieldsPerPage;
            for (var i = 0; i < loopLimit; i+= fieldsPerPage)
            {
                var embedBuilder = new LocalEmbedBuilder
                {
                    Fields = fields[i..Math.Min(i + fieldsPerPage, fields.Length)],
                    Color = Global.DefaultEmbedColor
                };

                if (_configuration.AutoGeneratePageTitles)
                    embedBuilder.WithTitle($"Page {_pages.Count + 1}/{PageCount}");

                _pages.Add(new Page(_configuration.Content, embedBuilder));
            }
        }

        public ValueTask<Page> GetPageAsync(PagedMenu menu)
            => new(_pages[menu.CurrentPageIndex]);
        
    }
}