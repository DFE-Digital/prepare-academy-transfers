using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Helpers;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class ParagraphBuilder : IParagraphBuilder
    {
        private readonly OpenXmlElement _parent;

        public ParagraphBuilder(OpenXmlElement parent)
        {
            _parent = parent;
        }

        public void AddText(string text)
        {
            AddText(new TextElement {Value = text});
        }

        public void AddText(TextElement text)
        {
            var run = new Run() {RunProperties = new RunProperties()};
            DocumentBuilderHelpers.AddTextToElement(run, text.Value);

            if (text.Bold)
            {
                run.RunProperties.Bold = new Bold();
            }

            if (!string.IsNullOrEmpty(text.FontSize))
            {
                run.RunProperties.FontSize = new FontSize {Val = text.FontSize};
            }

            _parent.AppendChild(run);
        }

        public void AddText(TextElement[] text)
        {
            foreach (var t in text)
            {
                AddText(t);
            }
        }
    }
}