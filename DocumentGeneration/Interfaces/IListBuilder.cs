using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces
{
    public interface IListBuilder
    {
        public void AddItem(TextElement item);
        public void AddItem(string item);
        public void AddItem(TextElement[] elements);
    }
}