using System;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces
{
    public interface ITableBuilder
    {
        public void AddRow(Action<ITableRowBuilder> action);
        public void SetBorderStyle(TableBorderStyle style);
    }
}