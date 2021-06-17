using System.Collections.Generic;

namespace DocumentGeneration
{
    public interface IXDocumentBuilder
    {
        public void AddParagraph(string text);

        public void AddHeading(string text, XDocumentHeadingBuilder.HeadingLevelOptions headingLevel);

        public void AddTable(List<List<string>> tableData);

        public void Build();
    }
}