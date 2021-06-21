using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Builders;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration
{
    public class DocumentBuilder : IDocumentBuilder
    {
        private readonly Stream _stream;
        private readonly WordprocessingDocument _document;
        private readonly Body _body;

        public DocumentBuilder(Stream stream)
        {
            _stream = stream;
            _document = WordprocessingDocument.Create(_stream, WordprocessingDocumentType.Document);
            _document.AddMainDocumentPart();
            _document.MainDocumentPart.Document = new Document(new Body());
            _body = _document.MainDocumentPart.Document.Body;
            SetCompatibilityMode();
            AppendSectionProperties();
        }

        public void AddParagraph(Action<IParagraphBuilder> action)
        {
            var paragraph = new Paragraph();
            var builder = new ParagraphBuilder(paragraph);

            action(builder);

            _body.AppendChild(paragraph);
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            var table = new Table();
            var builder = new TableBuilder(table);

            action(builder);

            _body.AppendChild(table);
        }

        public void AddHeading(Action<IHeadingBuilder> action)
        {
            var builder = new HeadingBuilder(_body);
            action(builder);
        }

        public void AddHeader(Action<IHeaderBuilder> action)
        {
            var headerPart = _document.MainDocumentPart.HeaderParts.First();
            
            var header = new Header();
            var builder = new HeaderBuilder(header);
            action(builder);
            header.Save(headerPart);
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

        private void AppendSectionProperties()
        {
            var props = new SectionProperties();
            AddHeaderToProperties(props);
            AddPageMargin(props);
            _body.AppendChild(props);
        }


        private void AddHeaderToProperties(SectionProperties props)
        {
            var mainDocumentPart = _document.MainDocumentPart;
            var headerPart = mainDocumentPart.AddNewPart<HeaderPart>();
            var headerPartId = mainDocumentPart.GetIdOfPart(headerPart);

            var headerReference = new HeaderReference
                {Id = headerPartId, Type = new EnumValue<HeaderFooterValues>(HeaderFooterValues.Default)};
            props.AppendChild(headerReference);
        }

        private static void AddPageMargin(SectionProperties props)
        {
            var pageMargin = new PageMargin
            {
                Top = 850,
                Bottom = 994,
                Left = 1080,
                Right = 1080
            };
            props.AppendChild(pageMargin);
        }
    }
}