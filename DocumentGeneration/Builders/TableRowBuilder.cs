using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class TableRowBuilder : ITableRowBuilder, IElementBuilder<TableRow>
    {
        private readonly TableRow _tableRow;

        public TableRowBuilder()
        {
            _tableRow = new TableRow();
        }

        public void AddCell(string text)
        {
            AddCell(new TextElement {Value = text});
        }

        public void AddCell(TextElement textElement, TableCellProperties properties = null)
        {
            var tableCell = new TableCell
            {
                TableCellProperties = properties
            };
            var paragraphBuilder = new ParagraphBuilder();
            paragraphBuilder.AddText(textElement);
            tableCell.AppendChild(paragraphBuilder.Build());
            _tableRow.AppendChild(tableCell);
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

        public TableRow Build()
        {
            return _tableRow;
        }
    }
}