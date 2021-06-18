using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Helpers;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class ParagraphBuilder : IParagraphBuilder
    {
        private readonly Paragraph _parent;

        public ParagraphBuilder(Paragraph parent)
        {
            _parent = parent;
            parent.ParagraphProperties ??= new ParagraphProperties();
            parent.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines
            {
                AfterAutoSpacing = OnOffValue.FromBoolean(true),
                BeforeAutoSpacing = OnOffValue.FromBoolean(true)
            };
        }

        public void AddText(TextElement text)
        {
            var run = new Run() {RunProperties = new RunProperties()};
            run.RunProperties.RunFonts = new RunFonts()
            {
                Ascii = "Arial",
                HighAnsi = "Arial",
                ComplexScript = "Arial"
            };

            DocumentBuilderHelpers.AddTextToElement(run, text.Value);

            if (text.Bold)
            {
                run.RunProperties.Bold = new Bold();
            }

            if (!string.IsNullOrEmpty(text.FontSize))
            {
                run.RunProperties.FontSize = new FontSize {Val = text.FontSize};
            }

            if (!string.IsNullOrEmpty(text.Colour))
            {
                run.RunProperties.Color = new Color {Val = text.Colour};
            }

            _parent.AppendChild(run);
        }

        public void AddText(string text)
        {
            AddText(new TextElement {Value = text});
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