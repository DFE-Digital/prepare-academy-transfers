using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;

namespace DocumentGeneration.Builders
{
    public abstract class ListBuilder : IListBuilder, IElementBuilder<List<Paragraph>>
    {
        protected OpenXmlElement Parent;
        protected NumberingDefinitionsPart NumberingDefinitionsPart;
        protected int NumId;
        private readonly List<Paragraph> _items;

        protected ListBuilder()
        {
            _items = new List<Paragraph>();
        }

        public void AddItem(Action<IParagraphBuilder> action)
        {
            var paragraph = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    NumberingProperties = new NumberingProperties(new List<OpenXmlElement>
                    {
                        new NumberingLevelReference {Val = 0},
                        new NumberingId {Val = NumId}
                    })
                }
            };
            var paragraphBuilder = new ParagraphBuilder(paragraph);
            action(paragraphBuilder);
            _items.Add(paragraphBuilder.Build());
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
                        new NumberingId {Val = NumId}
                    })
                }
            };
            var paragraphBuilder = new ParagraphBuilder(paragraph);
            paragraphBuilder.AddText(item);
            _items.Add(paragraphBuilder.Build());
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
                        new NumberingId {Val = NumId}
                    })
                }
            };
            var paragraphBuilder = new ParagraphBuilder(paragraph);
            paragraphBuilder.AddText(elements);
            _items.Add(paragraphBuilder.Build());
        }

        protected void AddNumberingDefinitions(AbstractNum abstractNum, NumberingInstance numberingInstance)
        {
            var numberingDefinitionCount = NumberingDefinitionsPart.Numbering.Descendants<AbstractNum>().Count();

            if (numberingDefinitionCount > 0)
            {
                var lastAbstract = NumberingDefinitionsPart.Numbering.Descendants<AbstractNum>().Last();
                NumberingDefinitionsPart.Numbering.InsertAfter(abstractNum, lastAbstract);

                var lastNumberingInstance = NumberingDefinitionsPart.Numbering.Descendants<NumberingInstance>().Last();
                NumberingDefinitionsPart.Numbering.InsertAfter(numberingInstance, lastNumberingInstance);
            }
            else
            {
                NumberingDefinitionsPart.Numbering.AppendChild(abstractNum);
                NumberingDefinitionsPart.Numbering.AppendChild(numberingInstance);
            }
        }

        public List<Paragraph> Build()
        {
            return _items;
        }
    }
}