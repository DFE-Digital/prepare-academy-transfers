using Dfe.PrepareTransfers.DocumentGeneration.Elements;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces.Parents
{
    public interface ITextParent
    {
        public void AddText(string text);
        public void AddText(TextElement textElement);
        public void AddText(TextElement[] text);
    }
}