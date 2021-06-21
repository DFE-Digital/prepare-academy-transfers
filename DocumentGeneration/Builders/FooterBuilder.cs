using System;
using DocumentFormat.OpenXml.Wordprocessing;
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
            var paragraph = new Paragraph();
            var builder = new ParagraphBuilder(paragraph);
            action(builder);
            _footer.AppendChild(paragraph);
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            var table = new Table();
            var builder = new TableBuilder(table);
            action(builder);
            _footer.AppendChild(table);
        }
    }
}