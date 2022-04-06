using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;

namespace DocumentGeneration.Interfaces
{
    public interface ITableRowBuilder
    {
        public void AddCell(string text);
        public void AddCell(TextElement textElement, TableCellProperties properties = null);
        
        public void AddCells(string[] text);
        public void AddCells(TextElement[] text);
    }
}