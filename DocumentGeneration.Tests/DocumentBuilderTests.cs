using System;
using System.Collections;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;
using Xunit;

namespace DocumentGeneration.Tests
{
    public class DocumentBuilderTests
    {
        public class ParagraphTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenAddingParagraphWithString_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(pBuilder => { pBuilder.AddText("Meow"); });
                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single((IEnumerable) paragraphs);
                Assert.Equal("Meow", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingParagraphWithTextObject_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(pBuilder =>
                {
                    pBuilder.AddText(new TextElement {Value = "Woof"});
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single(paragraphs);
                Assert.Empty(paragraphs[0].Descendants<Bold>());
                Assert.Equal("Woof", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingParagraphWithBoldTextObject_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(pBuilder =>
                {
                    pBuilder.AddText(new TextElement {Value = "Woof", Bold = true});
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single(paragraphs);
                Assert.Single(paragraphs[0].Descendants<Bold>());
                Assert.Equal("Woof", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingParagraphWithMultipleTextObjects_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(pBuilder =>
                {
                    var text = new[]
                    {
                        new TextElement {Value = "Meow"},
                        new TextElement {Value = "Woof", Bold = true},
                        new TextElement {Value = "Quack"}
                    };
                    pBuilder.AddText(text);
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single(paragraphs);
                Assert.Single(paragraphs[0].Descendants<Bold>());
                Assert.Equal(3, paragraphs[0].Descendants<Run>().Count());
                Assert.Equal("MeowWoofQuack", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingTextObjectsWithWhitespace_GeneratesParagraphWithPreservedWhitespace()
            {
                var documentBody = GenerateDocumentBody(pBuilder =>
                {
                    var text = new[]
                    {
                        new TextElement {Value = "Meow"},
                        new TextElement {Value = " Woof ", Bold = true},
                        new TextElement {Value = "Quack"}
                    };
                    pBuilder.AddText(text);
                });

                var texts = documentBody.Descendants<Text>().ToList();
                Assert.Equal(SpaceProcessingModeValues.Preserve, texts[0].Space.Value);
            }

            [Theory]
            [InlineData("Meow\r\nWoof")]
            [InlineData("Meow\nWoof")]
            [InlineData("Meow\rWoof")]
            public void GivenTextWithNewLine_CreatesTextSeparatedByNewLines(string text)
            {
                var documentBody = GenerateDocumentBody(pBuilder => { pBuilder.AddText(text); });

                var paragraph = documentBody.Descendants<Paragraph>().ToList()[0];
                var texts = paragraph.Descendants<Text>().ToList();
                Assert.Equal(2, texts.Count);
                Assert.Single(paragraph.Descendants<Break>());
            }
        }

        private static Body GenerateDocumentBody(Action<IParagraphBuilder> action)
        {
            Body documentBody;
            using (var ms = new MemoryStream())
            {
                var builder = new DocumentBuilder(ms);
                builder.AddParagraph(action);
                builder.Build();

                using (var doc = WordprocessingDocument.Open(ms, false))
                {
                    documentBody = doc.MainDocumentPart.Document.Body;
                }
            }

            return documentBody;
        }
    }
}