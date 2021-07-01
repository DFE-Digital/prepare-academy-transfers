using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class TableRowBuilder : ITableRowBuilder
    {
        private OpenXmlElement _parent;

        public TableRowBuilder(TableRow tableRow)
        {
            _parent = tableRow;
        }

        public void AddCell(string text)
        {
            AddCell(new TextElement {Value = text});
        }

        public void AddCell(TextElement textElement)
        {
            var tableCell = new TableCell();
            var paragraphBuilder = new ParagraphBuilder();
            paragraphBuilder.AddText(textElement);
            tableCell.AppendChild(paragraphBuilder.Build());
            _parent.AppendChild(tableCell);
        }

        public void AddCells(string[] text)
        {
            foreach (var t in text)
            {
                AddCell(t);
            }
        }

        public void AddCells(TextElement[] text)
        {
            foreach (var t in text)
            {
                AddCell(t);
            }
        }
    }
}