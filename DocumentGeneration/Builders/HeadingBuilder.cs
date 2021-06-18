using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class HeadingBuilder : IHeadingBuilder
    {
        private readonly OpenXmlElement _parent;
        private HeadingLevel _headingLevel;

        public HeadingBuilder(OpenXmlElement parent)
        {
            _parent = parent;
        }

        public void SetHeadingLevel(HeadingLevel level)
        {
            _headingLevel = level;
        }

        public void AddText(string text)
        {
            var paragraph = new Paragraph();
            var builder = new ParagraphBuilder(paragraph);

            var textElement = new TextElement
            {
                Value = text,
                FontSize = HeadingLevelToFontSize()
            };
            builder.AddText(textElement);
            _parent.AppendChild(paragraph);
        }

        public void AddText(TextElement text)
        {
            var paragraph = new Paragraph();
            var builder = new ParagraphBuilder(paragraph);
            text.FontSize = HeadingLevelToFontSize();
            builder.AddText(text);
            _parent.AppendChild(paragraph);
        }

        private string HeadingLevelToFontSize()
        {
            return _headingLevel switch
            {
                HeadingLevel.One => "60",
                HeadingLevel.Two => "40",
                HeadingLevel.Three => "30",
                _ => "30"
            };
        }
    }
}