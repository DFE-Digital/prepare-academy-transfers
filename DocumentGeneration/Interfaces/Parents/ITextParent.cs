using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces.Parents
{
    public interface ITextParent
    {
        public void AddText(string text);
        public void AddText(TextElement textElement);
        public void AddText(TextElement[] text);
    }
}