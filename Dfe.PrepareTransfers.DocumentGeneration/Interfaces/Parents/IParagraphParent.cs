using System;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces.Parents
{
    public interface IParagraphParent
    {
        public void AddParagraph(Action<IParagraphBuilder> action);
        public void AddParagraph(string text);
    }
}