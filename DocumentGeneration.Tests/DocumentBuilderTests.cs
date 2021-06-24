using System;
using System.Collections;
using System.Collections.Generic;
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

        private static GenerateDocumentResponse GenerateDocument(Action<IDocumentBuilder> action)
        {
            var ms = new MemoryStream();
            var builder = new DocumentBuilder(ms);
            action(builder);
            builder.Build();

            using var doc = WordprocessingDocument.Open(ms, false);

            return new GenerateDocumentResponse
            {
                Numbering = doc.MainDocumentPart.NumberingDefinitionsPart.Numbering,
                Headers = doc.MainDocumentPart.HeaderParts.Select(p => p.Header).ToList(),
                FooterParts = doc.MainDocumentPart.FooterParts.Select(p => p.Footer).ToList(),
                Body = doc.MainDocumentPart.Document.Body
            };
        }

        private class GenerateDocumentResponse
        {
            public Body Body { get; set; }
            public Numbering Numbering { get; set; }
            public List<Header> Headers { get; set; }
            public List<Footer> FooterParts { get; set; }
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
            public void GivenAddingParagraphWithItalicTextObject_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder =>
                    {
                        pBuilder.AddText(new TextElement {Value = "Woof", Italic = true});
                    });
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single(paragraphs);
                Assert.Single(paragraphs[0].Descendants<Italic>());
                Assert.Equal("Woof", paragraphs[0].InnerText);
            }

            [Fact]
            public void GivenAddingParagraphWithUnderlineTextObject_GeneratesParagraphWithText()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder =>
                    {
                        pBuilder.AddText(new TextElement {Value = "Woof", Underline = true});
                    });
                });

                var paragraphs = documentBody.Descendants<Paragraph>().ToList();
                Assert.Single(paragraphs);
                Assert.Single(paragraphs[0].Descendants<Underline>());
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

            [Theory]
            [InlineData(ParagraphJustification.Left, JustificationValues.Left)]
            [InlineData(ParagraphJustification.Center, JustificationValues.Center)]
            [InlineData(ParagraphJustification.Right, JustificationValues.Right)]
            public void GivenParagraphJustification_JustifiesParagraphCorrectly(
                ParagraphJustification paragraphJustification,
                JustificationValues expectedJustification)
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddParagraph(pBuilder =>
                    {
                        pBuilder.AddText("Meow");
                        pBuilder.Justify(paragraphJustification);
                    });
                });

                var paragraph = documentBody.Descendants<Paragraph>().First();
                var paragraphProperties = paragraph.ParagraphProperties;
                Assert.Equal(expectedJustification, paragraphProperties.Justification.Val.Value);
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
                Assert.Single(documentBody.Descendants<TableProperties>());
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
                Assert.Single(documentBody.Descendants<TableProperties>());
                Assert.Single(documentBody.Descendants<TableRow>());
                Assert.Single(documentBody.Descendants<TableCell>());
                Assert.Equal("test", table[0].InnerText);
            }

            [Fact]
            public void GivenAddingATableWithMultipleStringCells_GeneratesTable()
            {
                var tests = new[] {"test1", "test2"};

                var documentBody = GenerateDocumentBody(builder =>
                {
                    builder.AddTable(tBuilder => { tBuilder.AddRow(rBuilder => { rBuilder.AddCells(tests); }); });
                });

                var table = documentBody.Descendants<Table>().ToList();
                Assert.Single(table);
                Assert.Single(documentBody.Descendants<TableProperties>());
                Assert.Single(documentBody.Descendants<TableRow>());
                Assert.Equal(2, documentBody.Descendants<TableCell>().Count());

                var tableCells = documentBody.Descendants<TableCell>().ToList();

                Assert.Equal("test1", tableCells[0].InnerText);
                Assert.Equal("test2", tableCells[1].InnerText);
            }

            [Fact]
            public void GivenAddingATableWithMultipleTextCells_GeneratesTable()
            {
                var textElements = new[]
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
                Assert.Single(documentBody.Descendants<TableProperties>());
                Assert.Single(documentBody.Descendants<TableRow>());
                Assert.Equal(2, documentBody.Descendants<TableCell>().Count());

                var tableCells = documentBody.Descendants<TableCell>().ToList();

                Assert.Equal("test1", tableCells[0].InnerText);
                Assert.Equal("test2", tableCells[1].InnerText);
            }

            [Fact]
            public void GivenAddingATableByRows_GeneratesTheCorrectTable()
            {
                var documentBody = GenerateDocumentBody(builder =>
                {
                    var textElements = new[]
                    {
                        new[] {new TextElement {Value = "One"}, new TextElement {Value = "Two"}},
                        new[] {new TextElement {Value = "Three"}, new TextElement {Value = "Four"}}
                    };
                    builder.AddTable(textElements);
                });

                var tableRows = documentBody.Descendants<TableRow>().ToList();
                var tableCells = documentBody.Descendants<TableCell>().ToList();

                Assert.Equal(2, tableRows.Count);
                Assert.Equal(4, tableCells.Count);
                Assert.Equal("OneTwo", tableRows[0].InnerText);
                Assert.Equal("ThreeFour", tableRows[1].InnerText);
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

        public class HeaderTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenHeaderHasText_GeneratesHeaderForDocument()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddHeader(hBuilder => { hBuilder.AddParagraph(pBuilder => pBuilder.AddText("Meow")); });
                });

                Assert.Single(response.Headers);
                var header = response.Headers[0];
                Assert.Single(header.Descendants<Paragraph>());
                Assert.Equal("Meow", header.InnerText);
            }

            [Fact]
            public void GivenHeaderHasTable_GeneratesHeaderForDocument()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddHeader(hBuilder =>
                    {
                        hBuilder.AddTable(
                            tBuilder => tBuilder.AddRow(rBuilder => { rBuilder.AddCell("Meow"); }));
                    });
                });

                Assert.Single(response.Headers);
                var header = response.Headers[0];
                Assert.Single(header.Descendants<Table>());
                Assert.Single(header.Descendants<Paragraph>());
                Assert.Equal("Meow", header.InnerText);
            }

            [Fact]
            public void GivenAddingATableByRows_GeneratesTheCorrectTable()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddHeader(hBuilder =>
                    {
                        var textElements = new[]
                        {
                            new[] {new TextElement {Value = "One"}, new TextElement {Value = "Two"}},
                            new[] {new TextElement {Value = "Three"}, new TextElement {Value = "Four"}}
                        };
                        hBuilder.AddTable(textElements);
                    });
                });

                var header = response.Headers[0];

                var tableRows = header.Descendants<TableRow>().ToList();
                var tableCells = header.Descendants<TableCell>().ToList();

                Assert.Equal(2, tableRows.Count);
                Assert.Equal(4, tableCells.Count);
                Assert.Equal("OneTwo", tableRows[0].InnerText);
                Assert.Equal("ThreeFour", tableRows[1].InnerText);
            }
        }

        public class FooterTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenFooterHasText_GeneratesFooterForDocument()
            {
                using var ms = new MemoryStream();
                var builder = new DocumentBuilder(ms);
                builder.AddFooter(fBuilder => { fBuilder.AddParagraph(pBuilder => pBuilder.AddText("Meow")); });
                builder.Build();

                using var doc = WordprocessingDocument.Open(ms, false);
                var footers = doc.MainDocumentPart.FooterParts.ToList();

                Assert.Single(footers);
                var footer = footers[0].Footer;
                Assert.Single(footer.Descendants<Paragraph>());
                Assert.Equal("Meow", footer.InnerText);
            }

            [Fact]
            public void GivenFooterHasTable_GeneratesFooterForDocument()
            {
                using var ms = new MemoryStream();
                var builder = new DocumentBuilder(ms);
                builder.AddFooter(fBuilder =>
                {
                    fBuilder.AddTable(tBuilder => tBuilder.AddRow(rBuilder => { rBuilder.AddCell("Meow"); }));
                });
                builder.Build();

                using var doc = WordprocessingDocument.Open(ms, false);
                var footers = doc.MainDocumentPart.FooterParts.ToList();
                Assert.Single(footers);
                var footer = footers[0].Footer;
                Assert.Single(footer.Descendants<Table>());
                Assert.Single(footer.Descendants<Paragraph>());
                Assert.Equal("Meow", footer.InnerText);
            }

            [Fact]
            public void GivenAddingATableByRows_GeneratesTheCorrectTable()
            {
                using var ms = new MemoryStream();
                var builder = new DocumentBuilder(ms);
                builder.AddFooter(fBuilder =>
                {
                    var textElements = new[]
                    {
                        new[] {new TextElement {Value = "One"}, new TextElement {Value = "Two"}},
                        new[] {new TextElement {Value = "Three"}, new TextElement {Value = "Four"}}
                    };
                    fBuilder.AddTable(textElements);
                });
                builder.Build();

                using var doc = WordprocessingDocument.Open(ms, false);
                var footers = doc.MainDocumentPart.FooterParts.ToList();
                var footer = footers[0].Footer;

                var tableRows = footer.Descendants<TableRow>().ToList();
                var tableCells = footer.Descendants<TableCell>().ToList();

                Assert.Equal(2, tableRows.Count);
                Assert.Equal(4, tableCells.Count);
                Assert.Equal("OneTwo", tableRows[0].InnerText);
                Assert.Equal("ThreeFour", tableRows[1].InnerText);
            }
        }

        public class BulletedListTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenAddingOneBulletedList_CreatesASingleNumberingDefinitionAndAssignsThem()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddBulletedList(lBuilder => { lBuilder.AddItem("One"); });
                });

                var numbering = response.Numbering;
                var numberingFormats = numbering.Descendants<NumberingFormat>().ToList();
                var abstractNumDefinitions = numbering.Descendants<AbstractNum>().ToList();
                var numberingInstances = numbering.Descendants<NumberingInstance>().ToList();
                var paragraph = response.Body.Descendants<Paragraph>().First();

                Assert.Single(abstractNumDefinitions);
                Assert.Equal(NumberFormatValues.Bullet, numberingFormats[0].Val.Value);
                Assert.Equal(0, abstractNumDefinitions[0].AbstractNumberId.Value);
                Assert.Single(numberingInstances);
                Assert.Equal(0, (int) numberingInstances[0].AbstractNumId.Val);
                Assert.Equal(0, (int) paragraph.ParagraphProperties.NumberingProperties.NumberingId.Val);
            }

            [Fact]
            public void GivenAddingTwoBulletedLists_CreatesTwoNumberingDefinitions()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddBulletedList(lBuilder => { lBuilder.AddItem("One"); });
                    builder.AddBulletedList(lBuilder => { lBuilder.AddItem("Two"); });
                });

                var numbering = response.Numbering;
                var numberingFormats = numbering.Descendants<NumberingFormat>().ToList();
                var abstractNumDefinitions = numbering.Descendants<AbstractNum>().ToList();
                var numberingInstances = numbering.Descendants<NumberingInstance>().ToList();

                Assert.Equal(NumberFormatValues.Bullet, numberingFormats[0].Val.Value);
                Assert.Equal(NumberFormatValues.Bullet, numberingFormats[1].Val.Value);
                Assert.Equal(2, abstractNumDefinitions.Count);
                Assert.Equal(0, abstractNumDefinitions[0].AbstractNumberId.Value);
                Assert.Equal(1, abstractNumDefinitions[1].AbstractNumberId.Value);
                Assert.Equal(2, numberingInstances.Count);
                Assert.Equal(0, (int) numberingInstances[0].AbstractNumId.Val);
                Assert.Equal(1, (int) numberingInstances[1].AbstractNumId.Val);
            }

            [Fact]
            public void GivenAddingBulletedListWithStringItem_CreatesCorrectBulletedList()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddBulletedList(lBuilder =>
                    {
                        lBuilder.AddItem("One");
                        lBuilder.AddItem("Two");
                        lBuilder.AddItem("Three");
                    });
                });

                var paragraphs = response.Body.Descendants<Paragraph>().ToList();

                Assert.Equal(3, paragraphs.Count);
                Assert.True(paragraphs.All(p =>
                    p.ParagraphProperties.Descendants<NumberingProperties>().First().NumberingId.Val == 0));
                Assert.Equal("One", paragraphs[0].InnerText);
                Assert.Equal("Two", paragraphs[1].InnerText);
                Assert.Equal("Three", paragraphs[2].InnerText);
            }

            [Fact]
            public void GivenAddingBulletedListWithTextElementItem_CreatesCorrectBulletedList()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddBulletedList(lBuilder =>
                    {
                        lBuilder.AddItem(new TextElement("One"));
                        lBuilder.AddItem(new TextElement("Two"));
                        lBuilder.AddItem(new TextElement("Three"));
                    });
                });

                var paragraphs = response.Body.Descendants<Paragraph>().ToList();

                Assert.Equal(3, paragraphs.Count);
                Assert.True(paragraphs.All(p =>
                    p.ParagraphProperties.Descendants<NumberingProperties>().First().NumberingId.Val == 0));
                Assert.Equal("One", paragraphs[0].InnerText);
                Assert.Equal("Two", paragraphs[1].InnerText);
                Assert.Equal("Three", paragraphs[2].InnerText);
            }

            [Fact]
            public void GivenAddingBulletedListWithTextElementListItem_CreatesCorrectBulletedList()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddBulletedList(lBuilder =>
                    {
                        lBuilder.AddItem(new[] {new TextElement("One"), new TextElement("Two")});
                    });
                });

                var paragraphs = response.Body.Descendants<Paragraph>().ToList();

                Assert.Single(paragraphs);
                Assert.True(paragraphs.All(p =>
                    p.ParagraphProperties.Descendants<NumberingProperties>().First().NumberingId.Val == 0));
                Assert.Equal("OneTwo", paragraphs[0].InnerText);
            }
        }

        public class NumberedListTests : DocumentBuilderTests
        {
            [Fact]
            public void GivenAddingOneNumberedList_CreatesASingleNumberingDefinitionAndAssignsThem()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddNumberedList(lBuilder => { lBuilder.AddItem("One"); });
                });

                var numbering = response.Numbering;
                var numberingFormats = numbering.Descendants<NumberingFormat>().ToList();
                var abstractNumDefinitions = numbering.Descendants<AbstractNum>().ToList();
                var numberingInstances = numbering.Descendants<NumberingInstance>().ToList();
                var paragraph = response.Body.Descendants<Paragraph>().First();

                Assert.Single(abstractNumDefinitions);
                Assert.Equal(NumberFormatValues.Decimal, numberingFormats[0].Val.Value);
                Assert.Equal(0, abstractNumDefinitions[0].AbstractNumberId.Value);
                Assert.Single(numberingInstances);
                Assert.Equal(0, (int) numberingInstances[0].AbstractNumId.Val);
                Assert.Equal(0, (int) paragraph.ParagraphProperties.NumberingProperties.NumberingId.Val);
            }

            [Fact]
            public void GivenAddingTwoNumberedLists_CreatesTwoNumberingDefinitions()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddNumberedList(lBuilder => { lBuilder.AddItem("One"); });
                    builder.AddNumberedList(lBuilder => { lBuilder.AddItem("Two"); });
                });

                var numbering = response.Numbering;
                var numberingFormats = numbering.Descendants<NumberingFormat>().ToList();
                var abstractNumDefinitions = numbering.Descendants<AbstractNum>().ToList();
                var numberingInstances = numbering.Descendants<NumberingInstance>().ToList();

                Assert.Equal(NumberFormatValues.Decimal, numberingFormats[0].Val.Value);
                Assert.Equal(NumberFormatValues.Decimal, numberingFormats[1].Val.Value);
                Assert.Equal(2, abstractNumDefinitions.Count);
                Assert.Equal(0, abstractNumDefinitions[0].AbstractNumberId.Value);
                Assert.Equal(1, abstractNumDefinitions[1].AbstractNumberId.Value);
                Assert.Equal(2, numberingInstances.Count);
                Assert.Equal(0, (int) numberingInstances[0].AbstractNumId.Val);
                Assert.Equal(1, (int) numberingInstances[1].AbstractNumId.Val);
            }

            [Fact]
            public void GivenAddingNumberedListWithStringItem_CreatesCorrectNumberedList()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddNumberedList(lBuilder =>
                    {
                        lBuilder.AddItem("One");
                        lBuilder.AddItem("Two");
                        lBuilder.AddItem("Three");
                    });
                });

                var paragraphs = response.Body.Descendants<Paragraph>().ToList();

                Assert.Equal(3, paragraphs.Count);
                Assert.True(paragraphs.All(p =>
                    p.ParagraphProperties.Descendants<NumberingProperties>().First().NumberingId.Val == 0));
                Assert.Equal("One", paragraphs[0].InnerText);
                Assert.Equal("Two", paragraphs[1].InnerText);
                Assert.Equal("Three", paragraphs[2].InnerText);
            }

            [Fact]
            public void GivenAddingNumberedListWithTextElementItem_CreatesCorrectNumberedList()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddNumberedList(lBuilder =>
                    {
                        lBuilder.AddItem(new TextElement("One"));
                        lBuilder.AddItem(new TextElement("Two"));
                        lBuilder.AddItem(new TextElement("Three"));
                    });
                });

                var paragraphs = response.Body.Descendants<Paragraph>().ToList();

                Assert.Equal(3, paragraphs.Count);
                Assert.True(paragraphs.All(p =>
                    p.ParagraphProperties.Descendants<NumberingProperties>().First().NumberingId.Val == 0));
                Assert.Equal("One", paragraphs[0].InnerText);
                Assert.Equal("Two", paragraphs[1].InnerText);
                Assert.Equal("Three", paragraphs[2].InnerText);
            }

            [Fact]
            public void GivenAddingNumberedListWithTextElementListItem_CreatesCorrectNumberedList()
            {
                var response = GenerateDocument(builder =>
                {
                    builder.AddNumberedList(lBuilder =>
                    {
                        lBuilder.AddItem(new[] {new TextElement("One"), new TextElement("Two")});
                    });
                });

                var paragraphs = response.Body.Descendants<Paragraph>().ToList();

                Assert.Single(paragraphs);
                Assert.True(paragraphs.All(p =>
                    p.ParagraphProperties.Descendants<NumberingProperties>().First().NumberingId.Val == 0));
                Assert.Equal("OneTwo", paragraphs[0].InnerText);
            }
        }
    }
}