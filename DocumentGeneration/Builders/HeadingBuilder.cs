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
            AddText(new TextElement {Value = text});
        }

        public void AddText(TextElement text)
        {
            var paragraph = new Paragraph();
            var builder = new ParagraphBuilder(paragraph);
            text.FontSize = HeadingLevelToFontSize();
            text.Colour = "104f75";
            builder.AddText(text);
            _parent.AppendChild(paragraph);
        }

        private string HeadingLevelToFontSize()
        {
            return _headingLevel switch
            {
                HeadingLevel.One => "36",
                HeadingLevel.Two => "32",
                HeadingLevel.Three => "28",
                _ => "28"
            };
        }
    }
}