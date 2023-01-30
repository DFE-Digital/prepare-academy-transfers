using System;
using System.Collections.Generic;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces.Parents
{
    public interface ITableParent
    {
        public void AddTable(Action<ITableBuilder> action);
        public void AddTable(IEnumerable<TextElement[]> rows);
    }
}