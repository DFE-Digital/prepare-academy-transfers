using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces
{
    public interface IParagraphBuilder
    {
        public void AddText(string text);
        public void AddText(TextElement textElement);
        public void AddText(TextElement[] text);
        public void Justify(ParagraphJustification paragraphJustification);
    }
}