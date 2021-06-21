using System;

namespace DocumentGeneration.Interfaces.Parents
{
    public interface IParagraphParent
    {
        public void AddParagraph(Action<IParagraphBuilder> action);
    }
}