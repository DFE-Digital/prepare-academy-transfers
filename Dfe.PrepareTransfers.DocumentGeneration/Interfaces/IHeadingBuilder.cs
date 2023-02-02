using Dfe.PrepareTransfers.DocumentGeneration.Elements;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces
{
    public interface IHeadingBuilder
    {
        public void SetHeadingLevel(HeadingLevel level);
        public void AddText(string text);
        public void AddText(TextElement text);
    }
}