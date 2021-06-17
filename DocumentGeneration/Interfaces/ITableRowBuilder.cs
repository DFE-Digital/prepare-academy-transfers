using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces
{
    public interface ITableRowBuilder
    {
        public void AddCell(string text);
        public void AddCell(TextElement textElement);
        public void AddCells(string[] text);
        public void AddCells(TextElement[] text);
    }
}