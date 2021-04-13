using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentGeneration
{
    public class DocumentTableBuilder
    {
        private readonly OpenXmlElement _parentElement;
        private readonly Table _table;

        public DocumentTableBuilder(OpenXmlElement parentElement)
        {
            _parentElement = parentElement;
            _table = new Table();

            AddTableProperties();
        }

        private void AddTableProperties()
        {
            var tableProperties = new TableProperties
            {
                TableBorders = new TableBorders
                {
                    TopBorder = new TopBorder
                        {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                    RightBorder = new RightBorder
                        {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                    BottomBorder = new BottomBorder
                        {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                    LeftBorder = new LeftBorder
                        {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                    InsideVerticalBorder = new InsideVerticalBorder
                        {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                    InsideHorizontalBorder = new InsideHorizontalBorder
                        {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                }
            };

            _table.AppendChild(tableProperties);
        }

        public void AddTableRow(List<string> cellData)
        {
            var tableRow = new TableRow();

            cellData.ForEach(cellText =>
            {
                var tableCell = new TableCell();

                var paragraph = new Paragraph();
                var run = new Run(new Text(cellText));

                paragraph.AppendChild(run);
                tableCell.AppendChild(paragraph);
                tableRow.AppendChild(tableCell);
            });
            
            _table.AppendChild(tableRow);
        }

        public void Build()
        {
            _parentElement.AppendChild(_table);
        }
    }
}