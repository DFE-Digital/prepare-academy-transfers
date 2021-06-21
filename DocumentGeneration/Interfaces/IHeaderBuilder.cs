using System;

namespace DocumentGeneration.Interfaces
{
    public interface IHeaderBuilder
    {
        public void AddParagraph(Action<IParagraphBuilder> action);
    }
}