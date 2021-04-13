using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Extensions.Interactivity.Menus;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Npgsql.Replication.TestDecoding;

namespace CheeseBot.Disqord
{
    public class FieldBasedPageProvider : IPageProvider
    {
        private readonly List<Page> _pages;

        public int PageCount { get; }

        public FieldBasedPageProvider(IEnumerable<LocalEmbedFieldBuilder> fields, int fieldsPerPage)
        {
            _pages = new List<Page>();

            var fieldArray = fields as LocalEmbedFieldBuilder[] ?? fields.ToArray();
            var totalFields = fieldArray.Length;
            // this is ugly, but ambiguous calls and math.ceiling returning a double forced my hand
            PageCount = (int)Math.Ceiling((decimal)totalFields / fieldsPerPage);
            
            CreatePages(fieldArray, fieldsPerPage);
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

                _pages.Add(new Page(embedBuilder));
            }
        }

        public ValueTask<Page> GetPageAsync(PagedMenu menu)
        {
            return new(_pages[menu.CurrentPageIndex]);
        }
    }
}