using System;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces
{
    public interface IListBuilder
    {
        public void AddItem(TextElement item);
        public void AddItem(string item);
        public void AddItem(TextElement[] elements);
        public void AddItem(Action<IParagraphBuilder> action);
    }
}