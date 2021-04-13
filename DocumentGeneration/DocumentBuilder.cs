using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentGeneration
{
    public class DocumentBuilder
    {
        private readonly MemoryStream _memoryStream;
        private readonly WordprocessingDocument _document;
        private readonly Body _body;

        public DocumentBuilder(MemoryStream memoryStream)
        {
            _memoryStream = memoryStream;
            _document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);
            _document.AddMainDocumentPart();
            _document.MainDocumentPart.Document = new Document(new Body());
            _body = _document.MainDocumentPart.Document.Body;
        }

        public void AddParagraph(string text)
        {
            var paragraph = new Paragraph();
            var run = new Run() {RunProperties = new RunProperties()};
            var runProperties = run.RunProperties;
            var runText = new Text(text);

            run.AppendChild(runText);
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