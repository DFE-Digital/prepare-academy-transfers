using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Builders;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration
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

        public void AddNumberedList(Action<IListBuilder> action)
        {
            var builder = new NumberedListBuilder(_body, _document.MainDocumentPart.NumberingDefinitionsPart);
            action(builder);
        }

        public void AddBulletedList(Action<IListBuilder> action)
        {
            var builder = new BulletedListBuilder(_body, _document.MainDocumentPart.NumberingDefinitionsPart);
            action(builder);
        }

        public void AddParagraph(Action<IParagraphBuilder> action)
        {
            var builder = new ParagraphBuilder();
            action(builder);
            _body.AppendChild(builder.Build());
        }

        public void AddParagraph(string text)
        {
            var builder = new ParagraphBuilder();
            builder.AddText(text);
            _body.AppendChild(builder.Build());
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            var table = new Table();
            var builder = new TableBuilder(table);

            action(builder);

            _body.AppendChild(table);
        }

        public void AddTable(IEnumerable<TextElement[]> rows)
        {
            var table = new Table();
            var builder = new TableBuilder(table);
            foreach (var row in rows)
            {
                builder.AddRow(rBuilder => { rBuilder.AddCells(row); });
            }

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

        public void AddFooter(Action<IFooterBuilder> action)
        {
            var footerPart = _document.MainDocumentPart.FooterParts.First();

            var footer = new Footer();
            var builder = new FooterBuilder(footer);
            action(builder);
            footer.Save(footerPart);
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
            var texts = GetAllText();
            var properties = GetProperties<TDocument>();

            foreach (var text in texts)
            {
                var property = properties.FirstOrDefault(p =>
                    text.InnerText.Contains($"{p.GetCustomAttribute<DocumentTextAttribute>()?.Placeholder}",
                        StringComparison.OrdinalIgnoreCase));

                if (property == null) continue;

                var attribute = property.GetCustomAttribute<DocumentTextAttribute>();

                if (attribute == null) continue;

                text.Text = text.Text.Replace(
                    attribute.Placeholder,
                    property.GetValue(document)?.ToString(),
                    StringComparison.OrdinalIgnoreCase
                );
            }

            IEnumerable<Text> GetAllText()
            {
                var footerParts = _document.MainDocumentPart.FooterParts
                    .Where(fp => fp.Footer != null)
                    .SelectMany(fp => fp.Footer?.Descendants<Text>());
                return _document.MainDocumentPart.Document.Body.Descendants<Text>()
                    .Concat(footerParts)
                    .ToHashSet();
            }
        }

        private PropertyInfo[] GetProperties<TDocument>()
        {
            return typeof(TDocument).GetProperties()
                .Where(p => p.GetCustomAttribute<DocumentTextAttribute>() != null)
                .ToArray();
        }
    }
}