using System;

namespace DocumentGeneration.Interfaces.Parents
{
    public interface ITableParent
    {
        public void AddTable(Action<ITableBuilder> action);
    }
}