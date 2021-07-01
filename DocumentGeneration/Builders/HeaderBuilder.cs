using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class HeaderBuilder : IHeaderBuilder
    {
        private readonly Header _header;

        public HeaderBuilder(Header header)
        {
            _header = header;
        }

        public void AddParagraph(Action<IParagraphBuilder> action)
        {
            var builder = new ParagraphBuilder();
            action(builder);
            _header.AppendChild(builder.Build());
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            var table = new Table();
            var builder = new TableBuilder(table);
            action(builder);
            _header.AppendChild(table);
        }

        public void AddTable(IEnumerable<TextElement[]> rows)
        {
            var table = new Table();
            var builder = new TableBuilder(table);
            foreach (var row in rows)
            {
                builder.AddRow(rBuilder => { rBuilder.AddCells(row); });
            }

            _header.AppendChild(table);
        }
    }
}