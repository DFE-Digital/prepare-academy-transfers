using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces.Parents;

namespace DocumentGeneration.Interfaces
{
    public interface IParagraphBuilder : ITextParent
    {
        public void Justify(ParagraphJustification paragraphJustification);
    }
}