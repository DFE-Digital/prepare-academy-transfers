using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentGeneration.Builders
{
    public class NumberedListBuilder : ListBuilder
    {
        public NumberedListBuilder(NumberingDefinitionsPart numberingDefinitionsPart)
        {
            NumberingDefinitionsPart = numberingDefinitionsPart;
            NumId = AddNumberingDefinitions();
        }

        private int AddNumberingDefinitions()
        {
            var numberingDefinitionCount = NumberingDefinitionsPart.Numbering.Descendants<AbstractNum>().Count() + 1;

            var abstractNum = new AbstractNum(
                new Level(
                    new List<OpenXmlElement>
                    {
                        new StartNumberingValue {Val = 1},
                        new NumberingFormat
                            {Val = new EnumValue<NumberFormatValues>(NumberFormatValues.Decimal)},
                        new LevelText {Val = "%1."},
                        new LevelJustification
                            {Val = new EnumValue<LevelJustificationValues>(LevelJustificationValues.Left)},
                        new ParagraphProperties(
                            new Indentation {Left = "720", Hanging = "360"}
                        )
                    }
                ) {LevelIndex = 0}
            )
            {
                AbstractNumberId = numberingDefinitionCount
            };

            var numberingInstance = new NumberingInstance(new AbstractNumId {Val = numberingDefinitionCount})
            {
                NumberID = numberingDefinitionCount
            };

            AddNumberingDefinitions(abstractNum, numberingInstance);
            
            return numberingDefinitionCount;
        }
    }
}