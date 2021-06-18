using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces
{
    public interface IHeadingBuilder
    {
        public void SetHeadingLevel(HeadingLevel level);
        public void AddText(string text);
        public void AddText(TextElement text);
    }
}