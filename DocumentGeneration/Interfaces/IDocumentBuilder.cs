using System;

namespace DocumentGeneration.Interfaces
{
    public interface IDocumentBuilder
    {
        public void AddParagraph(Action<IParagraphBuilder> action);
        public void AddTable(Action<ITableBuilder> action);
        public void Build();
    }
}