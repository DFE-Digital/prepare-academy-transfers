using System.Collections.Generic;

namespace DocumentGeneration
{
    public interface XIDocumentBuilder
    {
        public void AddParagraph();

        public void AddHeading(string text, XDocumentHeadingBuilder.HeadingLevelOptions headingLevel);

        public void AddTable(List<List<string>> tableData);

        public void Build();
    }
}