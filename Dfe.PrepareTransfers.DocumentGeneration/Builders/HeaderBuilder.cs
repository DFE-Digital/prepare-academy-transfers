using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;
using Dfe.PrepareTransfers.DocumentGeneration.Interfaces;

namespace Dfe.PrepareTransfers.DocumentGeneration.Builders
{
    public class HeaderBuilder : IHeaderBuilder, IElementBuilder<Header>
    {
        private readonly Header _header;

        public HeaderBuilder()
        {
            _header = new Header();
        }

        public void AddParagraph(Action<IParagraphBuilder> action)
        {
            var builder = new ParagraphBuilder();
            action(builder);
            _header.AppendChild(builder.Build());
        }

        public void AddParagraph(string text)
        {
            var builder = new ParagraphBuilder();
            builder.AddText(text);
            _header.AppendChild(builder.Build());
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            var builder = new TableBuilder();
            action(builder);
            _header.AppendChild(builder.Build());
        }

        public void AddTable(IEnumerable<TextElement[]> rows)
        {
            var builder = new TableBuilder();
            foreach (var row in rows)
            {
                builder.AddRow(rBuilder => { rBuilder.AddCells(row); });
            }

            _header.AppendChild(builder.Build());
        }

        public Header Build()
        {
            return _header;
        }
    }
}