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
        private static Body GenerateDocumentBody(Action<IDocumentBuilder> action)
        {
            Body documentBody;
            using (var ms = new MemoryStream())
            {
                var builder = new DocumentBuilder(ms);
                action(builder);
                builder.Build();

                using (var doc = WordprocessingDocument.Open(ms, false))
                {
                    documentBody = doc.MainDocumentPart.Document.Body;
                }
            }

            return documentBody;
        }

        public class ParagraphTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenAddingParagraphWithString_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder => { pBuilder.AddText("Meow"); });
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single((IEnumerable) paragraphs);
                Assert.Equal("Meow", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingParagraphWithTextObject_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder => { pBuilder.AddText(new TextElement {Value = "Woof"}); });
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single(paragraphs);
                Assert.Empty(paragraphs[0].Descendants<Bold>());
                Assert.Equal("Woof", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingParagraphWithBoldTextObject_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder =>
                    {
                        pBuilder.AddText(new TextElement {Value = "Woof", Bold = true});
                    });
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single(paragraphs);
                Assert.Single(paragraphs[0].Descendants<Bold>());
                Assert.Equal("Woof", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingParagraphWithMultipleTextObjects_GeneratesParagraphWithText()
            {
                var text = new[]
                {
                    new TextElement {Value = "Meow"},
                    new TextElement {Value = "Woof", Bold = true},
                    new TextElement {Value = "Quack"}
                };

                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder => { pBuilder.AddText(text); });
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
                var text = new[]
                {
                    new TextElement {Value = "Meow"},
                    new TextElement {Value = " Woof ", Bold = true},
                    new TextElement {Value = "Quack"}
                };

                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder => { pBuilder.AddText(text); });
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
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder => { pBuilder.AddText(text); });
                });

                var paragraph = documentBody.Descendants<Paragraph>().ToList()[0];
                var texts = paragraph.Descendants<Text>().ToList();
                Assert.Equal(2, texts.Count);
                Assert.Single(paragraph.Descendants<Break>());
            }
        }

        public class TableTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenAddingATableWithSingleStringCell_GeneratesTable()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddTable(tBuilder => { tBuilder.AddRow(rBuilder => { rBuilder.AddCell("test"); }); });
                });

                var table = documentBody.Descendants<Table>().ToList();
                Assert.Single(table);
                Assert.Single(documentBody.Descendants<TableRow>());
                Assert.Single(documentBody.Descendants<TableCell>());
                Assert.Equal("test", table[0].InnerText);
            }

            [Fact]
            public void GivenAddingATableWithSingleTextCell_GeneratesTable()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddTable(tBuilder =>
                    {
                        tBuilder.AddRow(rBuilder => { rBuilder.AddCell(new TextElement {Value = "test"}); });
                    });
                });

                var table = documentBody.Descendants<Table>().ToList();
                Assert.Single(table);
                Assert.Single(documentBody.Descendants<TableRow>());
                Assert.Single(documentBody.Descendants<TableCell>());
                Assert.Equal("test", table[0].InnerText);
            }

            [Fact]
            public void GivenAddingATableWithMultipleStringCells_GeneratesTable()
            {
                var tests = new string[] {"test1", "test2"};

                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddTable(tBuilder => { tBuilder.AddRow(rBuilder => { rBuilder.AddCells(tests); }); });
                });

                var table = documentBody.Descendants<Table>().ToList();
                Assert.Single(table);
                Assert.Single(documentBody.Descendants<TableRow>());
                Assert.Equal(2, documentBody.Descendants<TableCell>().Count());

                var tableCells = documentBody.Descendants<TableCell>().ToList();

                Assert.Equal("test1", tableCells[0].InnerText);
                Assert.Equal("test2", tableCells[1].InnerText);
            }

            [Fact]
            public void GivenAddingATableWithMultipleTextCells_GeneratesTable()
            {
                var textElements = new TextElement[]
                {
                    new TextElement {Value = "test1"},
                    new TextElement {Value = "test2"}
                };

                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddTable(tBuilder =>
                    {
                        tBuilder.AddRow(rBuilder => { rBuilder.AddCells(textElements); });
                    });
                });

                var table = documentBody.Descendants<Table>().ToList();
                Assert.Single(table);
                Assert.Single(documentBody.Descendants<TableRow>());
                Assert.Equal(2, documentBody.Descendants<TableCell>().Count());

                var tableCells = documentBody.Descendants<TableCell>().ToList();

                Assert.Equal("test1", tableCells[0].InnerText);
                Assert.Equal("test2", tableCells[1].InnerText);
            }
        }

        public class HeadingTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenHeadingWithString_AddsTextToDocument()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddHeading(hBuilder => { hBuilder.AddText("Meow"); });
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single((IEnumerable) paragraphs);
                Assert.Equal("Meow", paragraphs[0].InnerText);
            }

            [Theory]
            [InlineData(HeadingLevel.One, "36")]
            [InlineData(HeadingLevel.Two, "32")]
            [InlineData(HeadingLevel.Three, "28")]
            public void GivenHeadingWithStringAndSize_AddsTextWithCorrectFontSize(HeadingLevel level,
                string expectedSize)
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(level);
                        hBuilder.AddText("Meow");
                    });
                });

                var run = documentBody.Descendants<Run>().ToList();
                Assert.Single(run);
                Assert.Equal(expectedSize, run[0].RunProperties.FontSize.Val);
            }

            [Fact]
            public void GivenHeadingWithTextElement_AddsTextToDocument()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddHeading(hBuilder => { hBuilder.AddText(new TextElement {Value = "Meow"}); });
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single((IEnumerable) paragraphs);
                Assert.Equal("Meow", paragraphs[0].InnerText);
            }

            [Theory]
            [InlineData(HeadingLevel.One, "36")]
            [InlineData(HeadingLevel.Two, "32")]
            [InlineData(HeadingLevel.Three, "28")]
            public void GivenHeadingWithTextElementAndSize_AddsTextWithCorrectFontSize(HeadingLevel level,
                string expectedSize)
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddHeading(hBuilder =>
                    {
                        hBuilder.SetHeadingLevel(level);
                        hBuilder.AddText(new TextElement {Value = "Meow"});
                    });
                });

                var run = documentBody.Descendants<Run>().ToList();
                Assert.Single(run);
                Assert.Equal(expectedSize, run[0].RunProperties.FontSize.Val);
            }
        }
    }
}