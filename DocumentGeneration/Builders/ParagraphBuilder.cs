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

            if (text.Italic)
            {
                run.RunProperties.Italic = new Italic();
            }

            if (text.Underline)
            {
                run.RunProperties.Underline = new Underline
                    {Val = new EnumValue<UnderlineValues>(UnderlineValues.Single), Color = "000000"};
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

        public void Justify(ParagraphJustification paragraphJustification)
        {
            var justification = new Justification();
            justification.Val = paragraphJustification switch
            {
                ParagraphJustification.Left => new EnumValue<JustificationValues>(JustificationValues.Left),
                ParagraphJustification.Center => new EnumValue<JustificationValues>(JustificationValues.Center),
                ParagraphJustification.Right => new EnumValue<JustificationValues>(JustificationValues.Right),
                _ => justification.Val
            };
            _parent.ParagraphProperties.Justification = justification;
        }
    }
}