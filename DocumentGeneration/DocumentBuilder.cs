using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentGeneration
{
    public class DocumentBuilder
    {
        private readonly WordprocessingDocument _document;
        private readonly Body _body;

        public DocumentBuilder(Stream stream)
        {
            _document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
            _document.AddMainDocumentPart();
            _document.MainDocumentPart.Document = new Document(new Body());
            _body = _document.MainDocumentPart.Document.Body;
        }

        public void AddHeading(string text, DocumentHeadingBuilder.HeadingLevelOptions headingLevel)
        {
            DocumentHeadingBuilder.AddHeadingToElement(_body, text, headingLevel);
        }
        
        public void AddParagraph(string text)
        {
            var paragraph = new Paragraph();
            var run = new Run {RunProperties = new RunProperties()};
            DocumentBuilderHelpers.AddTextToElement(run, text);

            paragraph.AppendChild(run);
            _body.AppendChild(paragraph);
        }

        public void AddTable(List<List<string>> tableData)
        {
            var tableBuilder = new DocumentTableBuilder(_body);
            tableData.ForEach(row => tableBuilder.AddTableRow(row));
            tableBuilder.Build();
        }

        public void Build()
        {
            _document.Save();
            _document.Close();
        }
    }
}