using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Helpers;

namespace DocumentGeneration
{
    public class XDocumentBuilder : IXDocumentBuilder
    {
        private readonly WordprocessingDocument _document;
        private readonly Body _body;

        public XDocumentBuilder(Stream stream)
        {
            _document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
            _document.AddMainDocumentPart();
            _document.MainDocumentPart.Document = new Document(new Body());
            _body = _document.MainDocumentPart.Document.Body;
            SetCompatibilityMode();
        }

        public void AddHeading(string text, XDocumentHeadingBuilder.HeadingLevelOptions headingLevel)
        {
            XDocumentHeadingBuilder.AddHeadingToElement(_body, text, headingLevel);
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
            var tableBuilder = new XDocumentTableBuilder(_body);
            tableData.ForEach(row => tableBuilder.AddTableRow(row));
            tableBuilder.Build();
        }

        public void Build()
        {
            _document.Save();
            _document.Close();
        }

        private void SetCompatibilityMode()
        {
            var mainPart = _document.MainDocumentPart;
            var settingsPart = mainPart.DocumentSettingsPart;

            if (settingsPart != null) return;

            settingsPart = mainPart.AddNewPart<DocumentSettingsPart>();
            settingsPart.Settings = new Settings(
                new Compatibility(
                    new CompatibilitySetting
                    {
                        Name = new EnumValue<CompatSettingNameValues>
                            (CompatSettingNameValues.CompatibilityMode),
                        Val = new StringValue("15"),
                        Uri = new StringValue
                            ("http://schemas.microsoft.com/office/word")
                    }
                )
            );
            settingsPart.Settings.Save();
        }
    }
}