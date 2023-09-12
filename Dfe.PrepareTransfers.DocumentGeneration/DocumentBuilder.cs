using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Dfe.PrepareTransfers.DocumentGeneration.Builders;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;
using Dfe.PrepareTransfers.DocumentGeneration.Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.DocumentGeneration.Interfaces;

namespace Dfe.PrepareTransfers.DocumentGeneration
{
    public class DocumentBuilder : IDocumentBuilder
    {
        private readonly WordprocessingDocument _document;
        private readonly Body _body;
        private readonly MemoryStream _ms;

        public static DocumentBuilder CreateFromTemplate<TDocument>(MemoryStream stream, TDocument document)
        {
            var builder = new DocumentBuilder(stream);
            builder.PopulateTemplateWithDocument(document);
            return builder;
        }

        public DocumentBuilder()
        {
            _ms = new MemoryStream();
            _document = WordprocessingDocument.Create(_ms, WordprocessingDocumentType.Document);
            _document.AddMainDocumentPart();
            _document.MainDocumentPart.Document = new Document(new Body());
            _body = _document.MainDocumentPart.Document.Body;
            SetCompatibilityMode();
            AddNumberingDefinitions();
            AppendSectionProperties();
        }

        private DocumentBuilder(MemoryStream ms)
        {
            _ms = ms;
            _document = WordprocessingDocument.Open(ms, true);
            _body = _document.MainDocumentPart.Document.Body;
        }

        private void AddNumberingDefinitions()
        {
            var part = _document.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();
            part.Numbering = new Numbering();
        }

        public void ReplacePlaceholderWithContent(string placeholderText, Action<DocumentBodyBuilder> action)
        {
            var placeholderElement = _body
                .Descendants<Paragraph>()
                .First(element => element.InnerText == $"[{placeholderText}]");

            var builder = new DocumentBodyBuilder(_document, placeholderElement);
            action(builder);
            placeholderElement.Remove();
        }

        public void AddNumberedList(Action<IListBuilder> action)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddNumberedList(action);
        }

        public void AddBulletedList(Action<IListBuilder> action)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddBulletedList(action);
        }

        public void AddParagraph(Action<IParagraphBuilder> action)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddParagraph(action);
        }

        public void AddParagraph(string text)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddParagraph(text);
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddTable(action);
        }

        public void AddTable(IEnumerable<TextElement[]> rows)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddTable(rows);
        }

        public void AddHeading(Action<IHeadingBuilder> action)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddHeading(action);
        }

         public void AddTextHeading(string headingText, HeadingLevel headingLevel)
        {
            var builder = new DocumentBodyBuilder(_document, _body.ChildElements.Last());
            builder.AddTextHeading(headingText, headingLevel);
        }

        public void AddHeader(Action<IHeaderBuilder> action)
        {
            var headerPart = _document.MainDocumentPart.HeaderParts.First();

            var builder = new HeaderBuilder();
            action(builder);
            builder.Build().Save(headerPart);
        }

        public void AddFooter(Action<IFooterBuilder> action)
        {
            var footerPart = _document.MainDocumentPart.FooterParts.First();

            var builder = new FooterBuilder();
            action(builder);
            builder.Build().Save(footerPart);
        }

        public byte[] Build()
        {
            _document.Save();
            _document.Close();
            return _ms.ToArray();
        }

        private void SetCompatibilityMode()
        {
            var mainPart = _document.MainDocumentPart;
            var settingsPart = mainPart.DocumentSettingsPart;

            if (settingsPart != null)
            {
                return;
            }

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
            AddFooterToProperties(props);
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

        private void AddFooterToProperties(SectionProperties props)
        {
            var mainDocumentPart = _document.MainDocumentPart;
            var footerPart = mainDocumentPart.AddNewPart<FooterPart>();
            var footerPartId = mainDocumentPart.GetIdOfPart(footerPart);

            var footerReference = new FooterReference
                {Id = footerPartId, Type = new EnumValue<HeaderFooterValues>(HeaderFooterValues.Default)};
            props.AppendChild(footerReference);
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

        private void PopulateTemplateWithDocument<TDocument>(TDocument document)
        {
            const string placeholderDefaultFontSize = "24";
            var allParagraphs = GetAllParagraphs();
            var properties = GetProperties<TDocument>();

            foreach (var paragraph in allParagraphs)
            {
                var property = properties.FirstOrDefault(p =>
                    paragraph.InnerText.Contains($"{p.GetCustomAttribute<DocumentTextAttribute>()?.Placeholder}",
                        StringComparison.OrdinalIgnoreCase));

                if (property == null)
                {
                    continue;
                }

                var attribute = property.GetCustomAttribute<DocumentTextAttribute>();

                if (attribute == null)
                {
                    continue;
                }

                foreach (var paragraphChildElement in paragraph.ChildElements
                             .Where(paragraphChildElement => paragraphChildElement.GetType() != typeof(ParagraphProperties)).ToList())
                {
                    paragraph.RemoveChild(paragraphChildElement);
                }
                
                var val = property.GetValue(document)?.ToString();
                var run = new Run
                {
                    RunProperties = new RunProperties
                    {
                        FontSize = new FontSize { Val = placeholderDefaultFontSize }
                    }
                };
                DocumentBuilderHelpers.AddTextToElement(run, val);
                paragraph.AppendChild(run);
            }

            IEnumerable<Paragraph> GetAllParagraphs()
            {
                var footerParts = _document.MainDocumentPart.FooterParts
                    .Where(fp => fp.Footer != null)
                    .SelectMany(fp => fp.Footer?.Descendants<Paragraph>());
                return _document.MainDocumentPart.Document.Body.Descendants<Paragraph>()
                    .Concat(footerParts)
                    .ToHashSet();
            }
        }

        private static PropertyInfo[] GetProperties<TDocument>()
        {
            return typeof(TDocument).GetProperties()
                .Where(p => p.GetCustomAttribute<DocumentTextAttribute>() != null)
                .ToArray();
        }
    }
}