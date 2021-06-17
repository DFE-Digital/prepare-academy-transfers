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
        }

        public void AddRow(Action<ITableRowBuilder> action)
        {
            var tableRow = new TableRow();
            var tableRowBuilder = new TableRowBuilder(tableRow);

            action(tableRowBuilder);

            _parent.AppendChild(tableRow);
        }
    }
}
