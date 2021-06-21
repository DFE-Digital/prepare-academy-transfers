using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Interfaces;
using System;

namespace DocumentGeneration.Builders
{
    public class TableBuilder : ITableBuilder
    {
        private readonly OpenXmlElement _parent;

        public TableBuilder(OpenXmlElement parent)
        {
            _parent = parent;
            AddDefaultTableProperties();
        }

        public void AddRow(Action<ITableRowBuilder> action)
        {
            var tableRow = new TableRow();
            var tableRowBuilder = new TableRowBuilder(tableRow);

            action(tableRowBuilder);

            _parent.AppendChild(tableRow);
        }
        
        private void AddDefaultTableProperties()
        {
            var tableProperties = new TableProperties
            {
                TableBorders = new TableBorders
                {
                    TopBorder = new TopBorder
                        { Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0 },
                    RightBorder = new RightBorder
                        { Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0 },
                    BottomBorder = new BottomBorder
                        { Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0 },
                    LeftBorder = new LeftBorder
                        { Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0 },
                    InsideVerticalBorder = new InsideVerticalBorder
                        { Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0 },
                    InsideHorizontalBorder = new InsideHorizontalBorder
                        { Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0 },
                },
                TableCellMarginDefault = new TableCellMarginDefault
                {
                    TopMargin = new TopMargin { Width = "40" },
                    BottomMargin = new BottomMargin { Width = "40" },
                    TableCellRightMargin = new TableCellRightMargin { Width = 40 },
                    TableCellLeftMargin = new TableCellLeftMargin { Width = 40 }
                },
                TableLayout = new TableLayout
                {
                    Type = TableLayoutValues.Fixed,
                },
                TableWidth = new TableWidth
                {
                    Type = TableWidthUnitValues.Dxa,
                    Width = "10000"
                }
            };

            _parent.AppendChild(tableProperties);
        }
    }
}
