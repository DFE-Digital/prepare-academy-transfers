using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Interfaces;
using System;
using DocumentGeneration.Elements;

namespace DocumentGeneration.Builders
{
    public class TableBuilder : ITableBuilder
    {
        private readonly Table _parent;
        private TableProperties _tableProperties;

        public TableBuilder(Table parent)
        {
            _parent = parent;
            _tableProperties = new TableProperties();
            _parent.AppendChild(_tableProperties);
            AddDefaultTableProperties();
        }

        public void AddRow(Action<ITableRowBuilder> action)
        {
            var tableRow = new TableRow();
            var tableRowBuilder = new TableRowBuilder(tableRow);

            action(tableRowBuilder);

            _parent.AppendChild(tableRow);
        }

        public void SetBorderStyle(TableBorderStyle style)
        {
            _tableProperties.TableBorders = style switch
            {
                TableBorderStyle.Solid => SolidTableBorders(),
                TableBorderStyle.None => NoneTableBorders(),
                _ => _tableProperties.TableBorders
            };
        }

        private TableBorders NoneTableBorders()
        {
            return new TableBorders();
        }

        private void AddDefaultTableProperties()
        {
            _tableProperties.TableBorders = SolidTableBorders();
            _tableProperties.TableCellMarginDefault = new TableCellMarginDefault
            {
                TopMargin = new TopMargin {Width = "40"},
                BottomMargin = new BottomMargin {Width = "40"},
                TableCellRightMargin = new TableCellRightMargin {Width = 40},
                TableCellLeftMargin = new TableCellLeftMargin {Width = 40}
            };
            _tableProperties.TableLayout = new TableLayout {Type = TableLayoutValues.Fixed,};
            _tableProperties.TableWidth = new TableWidth {Type = TableWidthUnitValues.Dxa, Width = "10000"};
        }

        private static TableBorders SolidTableBorders()
        {
            return new TableBorders
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
            };
        }
    }
}