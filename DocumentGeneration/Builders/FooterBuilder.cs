using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class FooterBuilder : IFooterBuilder
    {
        private readonly Footer _footer;

        public FooterBuilder(Footer footer)
        {
            _footer = footer;
        }

        public void AddParagraph(Action<IParagraphBuilder> action)
        {
            var builder = new ParagraphBuilder();
            action(builder);
            _footer.AppendChild(builder.Build());
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            var table = new Table();
            var builder = new TableBuilder(table);
            action(builder);
            _footer.AppendChild(table);
        }

        public void AddTable(IEnumerable<TextElement[]> rows)
        {
            var table = new Table();
            var builder = new TableBuilder(table);
            foreach (var row in rows)
            {
                builder.AddRow(rBuilder => { rBuilder.AddCells(row); });
            }

            _footer.AppendChild(table);
        }
    }
}