using System;

namespace DocumentGeneration.Interfaces
{
    public interface ITableBuilder
    {
        public void AddRow(Action<ITableRowBuilder> action);
    }
}