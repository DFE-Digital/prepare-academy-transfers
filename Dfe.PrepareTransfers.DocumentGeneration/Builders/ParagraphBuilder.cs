using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;
using Dfe.PrepareTransfers.DocumentGeneration.Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.DocumentGeneration.Interfaces;

namespace Dfe.PrepareTransfers.DocumentGeneration.Builders
{
    public class ParagraphBuilder : IParagraphBuilder, IElementBuilder<Paragraph>
    {
        private readonly Paragraph _parent;
        private readonly List<Run> _runs;

        public ParagraphBuilder()
        {
            _parent = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    SpacingBetweenLines = new SpacingBetweenLines
                    {
                        AfterAutoSpacing = OnOffValue.FromBoolean(true),
                        BeforeAutoSpacing = OnOffValue.FromBoolean(true)
                    }
                }
            };
            _runs = new List<Run>();
        }

        public ParagraphBuilder(Paragraph parent)
        {
            _parent = parent;
            parent.ParagraphProperties ??= new ParagraphProperties();
            parent.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines
            {
                AfterAutoSpacing = OnOffValue.FromBoolean(true),
                BeforeAutoSpacing = OnOffValue.FromBoolean(true)
            };
            _runs = new List<Run>();
        }

        public void AddText(TextElement textElement)
        {
            const string defaultFontSize = "24";
            var run = new Run
            {
               RunProperties = new RunProperties
               {
                  RunFonts = new RunFonts()
                  {
                     Ascii = "Arial",
                     HighAnsi = "Arial",
                     ComplexScript = "Arial"
                  }
               }
            };

            DocumentBuilderHelpers.AddTextToElement(run, textElement.Value);

            if (textElement.Bold)
            {
                run.RunProperties.Bold = new Bold();
            }

            if (textElement.Italic)
            {
                run.RunProperties.Italic = new Italic();
            }

            if (textElement.Underline)
            {
                run.RunProperties.Underline = new Underline
                    { Val = new EnumValue<UnderlineValues>(UnderlineValues.Single), Color = "000000" };
            }

            run.RunProperties.FontSize = new FontSize { Val = textElement.FontSize ?? defaultFontSize };

            if (!string.IsNullOrEmpty(textElement.Colour))
            {
                run.RunProperties.Color = new Color { Val = textElement.Colour };
            }

            _runs.Add(run);
        }

        public void AddText(string text)
        {
            AddText(new TextElement { Value = text });
        }

        public void AddText(TextElement[] text)
        {
            foreach (var t in text)
            {
                AddText(t);
            }
        }

        public void AddNewLine()
        {
            var run = new Run(new Break());
            _runs.Add(run);
        }

        public void AddPageBreak()
        {
            _parent.ParagraphProperties = new ParagraphProperties()
            {
                PageBreakBefore = new PageBreakBefore()
            };
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

        public Paragraph Build()
        {
            _parent.Append(_runs);
            return _parent;
        }
    }
}