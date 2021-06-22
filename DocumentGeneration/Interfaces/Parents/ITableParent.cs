using System;
using System.Collections.Generic;
using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces.Parents
{
    public interface ITableParent
    {
        public void AddTable(Action<ITableBuilder> action);
        public void AddTable(IEnumerable<TextElement[]> rows);
    }
}