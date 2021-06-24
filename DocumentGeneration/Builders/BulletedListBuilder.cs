using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class BulletedListBuilder : IListBuilder
    {
        private readonly OpenXmlElement _parent;
        private readonly NumberingDefinitionsPart _numberingDefinitionsPart;
        private readonly int _numId;

        public BulletedListBuilder(OpenXmlElement parent, NumberingDefinitionsPart numberingDefinitionsPart)
        {
            _parent = parent;
            _numberingDefinitionsPart = numberingDefinitionsPart;
            _numId = AddNumberingDefinitions();
        }

        public void AddItem(string item)
        {
            AddItem(new TextElement(item));
        }

        public void AddItem(TextElement item)
        {
            var paragraph = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    NumberingProperties = new NumberingProperties(new List<OpenXmlElement>
                    {
                        new NumberingLevelReference {Val = 0},
                        new NumberingId {Val = _numId}
                    })
                }
            };
            var paragraphBuilder = new ParagraphBuilder(paragraph);
            paragraphBuilder.AddText(item);
            _parent.AppendChild(paragraph);
        }

        public void AddItem(TextElement[] elements)
        {
            var paragraph = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    NumberingProperties = new NumberingProperties(new List<OpenXmlElement>
                    {
                        new NumberingLevelReference {Val = 0},
                        new NumberingId {Val = _numId}
                    })
                }
            };
            var paragraphBuilder = new ParagraphBuilder(paragraph);
            paragraphBuilder.AddText(elements);
            _parent.AppendChild(paragraph);
        }

        private int AddNumberingDefinitions()
        {
            var numberingDefinitionCount = _numberingDefinitionsPart.Numbering.Descendants<AbstractNum>().Count();

            var numberingDefinition = new List<OpenXmlElement>
            {
                new AbstractNum(
                    new Level(
                        new List<OpenXmlElement>
                        {
                            new StartNumberingValue {Val = 1},
                            new NumberingFormat
                                {Val = new EnumValue<NumberFormatValues>(NumberFormatValues.Bullet)},
                            new LevelText {Val = ""},
                            new LevelJustification
                                {Val = new EnumValue<LevelJustificationValues>(LevelJustificationValues.Left)},
                            new ParagraphProperties(
                                new Indentation {Left = "720", Hanging = "360"}
                            ),
                            new RunProperties(
                                new RunFonts
                                {
                                    Ascii = "Symbol", HighAnsi = "Symbol", ComplexScript = "Symbol",
                                    Hint = new EnumValue<FontTypeHintValues>(FontTypeHintValues.Default)
                                }
                            )
                        }
                    ) {LevelIndex = 0}
                )
                {
                    AbstractNumberId = numberingDefinitionCount
                },
                new NumberingInstance(new AbstractNumId {Val = numberingDefinitionCount})
                {
                    NumberID = numberingDefinitionCount
                }
            };

            _numberingDefinitionsPart.Numbering.AppendChild(numberingDefinition[0]);
            _numberingDefinitionsPart.Numbering.AppendChild(numberingDefinition[1]);

            return numberingDefinitionCount;
        }
    }
}