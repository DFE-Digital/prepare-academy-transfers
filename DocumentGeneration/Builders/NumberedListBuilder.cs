using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Helpers;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public class NumberedListBuilder : IListBuilder
    {
        private readonly OpenXmlElement _parent;
        private readonly NumberingDefinitionsPart _numberingDefinitionsPart;

        public NumberedListBuilder(OpenXmlElement parent, NumberingDefinitionsPart numberingDefinitionsPart)
        {
            _parent = parent;
            _numberingDefinitionsPart = numberingDefinitionsPart;
        }

        public void AddItems(string[] items)
        {
            var numId = AddNumberingDefinitions();
            foreach (var item in items)
            {
                var paragraph = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        NumberingProperties = new NumberingProperties(new List<OpenXmlElement>
                        {
                            new NumberingLevelReference {Val = 0},
                            new NumberingId {Val = numId}
                        })
                    }
                };
                var run = new Run();
                DocumentBuilderHelpers.AddTextToElement(run, item);
                paragraph.AppendChild(run);
                _parent.AppendChild(paragraph);
            }
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
                                {Val = new EnumValue<NumberFormatValues>(NumberFormatValues.Decimal)},
                            new LevelText {Val = "%1."},
                            new LevelJustification
                                {Val = new EnumValue<LevelJustificationValues>(LevelJustificationValues.Left)}
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