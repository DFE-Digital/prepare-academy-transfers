using System;
using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces
{
    public interface ITableBuilder
    {
        public void AddRow(Action<ITableRowBuilder> action);
        public void SetBorderStyle(TableBorderStyle style);
    }
}