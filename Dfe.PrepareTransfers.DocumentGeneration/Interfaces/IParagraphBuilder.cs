using Dfe.PrepareTransfers.DocumentGeneration.Elements;
using Dfe.PrepareTransfers.DocumentGeneration.Interfaces.Parents;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces
{
    public interface IParagraphBuilder : ITextParent
    {
        public void AddNewLine();
        public void Justify(ParagraphJustification paragraphJustification);
        public void AddPageBreak();
    }
}