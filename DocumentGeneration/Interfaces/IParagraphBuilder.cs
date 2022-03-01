using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces.Parents;

namespace DocumentGeneration.Interfaces
{
    public interface IParagraphBuilder : ITextParent
    {
        public void AddNewLine();
        public void Justify(ParagraphJustification paragraphJustification);
        public void AddPageBreak();
    }
}