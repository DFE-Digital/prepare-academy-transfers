using System;
using System.Collections.Generic;
using System.Linq;
using DocumentGeneration.Builders;
using DocumentGeneration.Elements;
using DocumentGeneration.Helpers;
using DocumentGeneration.Interfaces;
using Xunit;

namespace DocumentGeneration.Tests.Helpers
{
    public class HtmlToDocumentTests
    {
        private readonly DocumentBuilderFake _builder;

        public HtmlToDocumentTests()
        {
            _builder = new DocumentBuilderFake();
        }

        private static string GetParagraphText(List<TextElement> paragraphElements)
        {
            return paragraphElements.Select(t => t.Value).Aggregate((acc, t) => acc + t);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GivenEmptyOrNull_BuildsNothing(string text)
        {
            HtmlToDocument.Convert(_builder, text);

            Assert.Empty(_builder.AddedParagraphs);
            Assert.Empty(_builder.AddedLists);
        }

        [Theory]
        [InlineData("Meow woof quack")]
        [InlineData("Moo quack cluck")]
        [InlineData("     ")]
        public void GivenParagraph_GeneratesParagraph(string text)
        {
            var html = $"<p>{text}</p>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedParagraphs);
            Assert.Equal(text, _builder.AddedParagraphs[0][0].Value);
        }

        [Fact]
        public void GivenParagraphWithMixedFormatting_GeneratesParagraphs()
        {
            const string html = "<p>Meow <b>Woof <u>Quack <i>Moo</i></u></b></p>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedParagraphs);
            var paragraphElements = _builder.AddedParagraphs[0];

            Assert.Equal("Meow Woof Quack Moo", GetParagraphText(paragraphElements));
            Assert.True(paragraphElements[1].Bold);
            Assert.True(paragraphElements[2].Bold);
            Assert.True(paragraphElements[2].Underline);
            Assert.True(paragraphElements[3].Bold);
            Assert.True(paragraphElements[3].Underline);
            Assert.True(paragraphElements[3].Italic);
        }

        [Fact]
        public void GivenParagraphContainsABreak_GenerateParagraphWithNewline()
        {
            const string html = "<p>Meow<br>Woof</p>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedParagraphs);
            var paragraphElements = _builder.AddedParagraphs[0];

            Assert.Equal("Meow\nWoof", GetParagraphText(paragraphElements));
        }

        [Fact]
        public void GivenOrderedListWithSingleListItem_RendersNumberedList()
        {
            const string html = "<ol><li>Meow</li></ol>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedLists);
            var list = _builder.AddedLists[0];
            Assert.Equal("number", list.Type);
            Assert.Single(list.AddedItems);
            Assert.Single(list.AddedItems[0]);
            Assert.Equal("Meow", list.AddedItems[0][0].Value);
        }

        [Fact]
        public void GivenOrderedListWithSeveralListItems_RendersNumberedList()
        {
            const string html = "<ol><li>Meow</li><li>Woof</li><li>Quack</li></ol>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedLists);
            var list = _builder.AddedLists[0];
            Assert.Equal("number", list.Type);
            Assert.Equal(3, list.AddedItems.Count);
            Assert.Single(list.AddedItems[0]);
            Assert.Single(list.AddedItems[1]);
            Assert.Single(list.AddedItems[2]);
            Assert.Equal("Meow", list.AddedItems[0][0].Value);
            Assert.Equal("Woof", list.AddedItems[1][0].Value);
            Assert.Equal("Quack", list.AddedItems[2][0].Value);
        }

        [Fact]
        public void GivenOrderedListWithSeveralListItemsWithMixedFormatting_RendersNumberedList()
        {
            const string html =
                "<ol><li><b>Me<i>ow</i></b></li><li>W<u>oo</u>f</li><li><b><u><i>Quack</b></u></i></li></ol>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedLists);
            var list = _builder.AddedLists[0];
            Assert.Equal("number", list.Type);
            Assert.Equal(3, list.AddedItems.Count);

            var firstItem = list.AddedItems[0];
            Assert.Equal(2, firstItem.Count);
            Assert.Equal("Me", firstItem[0].Value);
            Assert.True(firstItem[0].Bold);
            Assert.Equal("ow", firstItem[1].Value);
            Assert.True(firstItem[1].Bold);
            Assert.True(firstItem[1].Italic);

            var secondItem = list.AddedItems[1];
            Assert.Equal(3, secondItem.Count);
            Assert.Equal("W", secondItem[0].Value);
            Assert.Equal("oo", secondItem[1].Value);
            Assert.True(secondItem[1].Underline);
            Assert.Equal("f", secondItem[2].Value);

            var thirdItem = list.AddedItems[2];
            Assert.Single(thirdItem);
            Assert.Equal("Quack", thirdItem[0].Value);
            Assert.True(thirdItem[0].Bold);
            Assert.True(thirdItem[0].Italic);
            Assert.True(thirdItem[0].Underline);
        }

        [Fact]
        public void GivenUnorderedListWithSingleListItem_RendersBulletList()
        {
            const string html = "<ul><li>Meow</li></ul>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedLists);
            var list = _builder.AddedLists[0];
            Assert.Equal("bullet", list.Type);
            Assert.Single(list.AddedItems);
            Assert.Single(list.AddedItems[0]);
            Assert.Equal("Meow", list.AddedItems[0][0].Value);
        }

        [Fact]
        public void GivenUnorderedListWithSeveralListItems_RendersBulletList()
        {
            const string html = "<ul><li>Meow</li><li>Woof</li><li>Quack</li></ul>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedLists);
            var list = _builder.AddedLists[0];
            Assert.Equal("bullet", list.Type);
            Assert.Equal(3, list.AddedItems.Count);
            Assert.Single(list.AddedItems[0]);
            Assert.Single(list.AddedItems[1]);
            Assert.Single(list.AddedItems[2]);
            Assert.Equal("Meow", list.AddedItems[0][0].Value);
            Assert.Equal("Woof", list.AddedItems[1][0].Value);
            Assert.Equal("Quack", list.AddedItems[2][0].Value);
        }

        [Fact]
        public void GivenUnorderedListWithSeveralListItemsWithMixedFormatting_RendersBulletList()
        {
            const string html =
                "<ul><li><b>Me<i>ow</i></b></li><li>W<u>oo</u>f</li><li><b><u><i>Quack</b></u></i></li></ul>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedLists);
            var list = _builder.AddedLists[0];
            Assert.Equal("bullet", list.Type);
            Assert.Equal(3, list.AddedItems.Count);

            var firstItem = list.AddedItems[0];
            Assert.Equal(2, firstItem.Count);
            Assert.Equal("Me", firstItem[0].Value);
            Assert.True(firstItem[0].Bold);
            Assert.Equal("ow", firstItem[1].Value);
            Assert.True(firstItem[1].Bold);
            Assert.True(firstItem[1].Italic);

            var secondItem = list.AddedItems[1];
            Assert.Equal(3, secondItem.Count);
            Assert.Equal("W", secondItem[0].Value);
            Assert.Equal("oo", secondItem[1].Value);
            Assert.True(secondItem[1].Underline);
            Assert.Equal("f", secondItem[2].Value);

            var thirdItem = list.AddedItems[2];
            Assert.Single(thirdItem);
            Assert.Equal("Quack", thirdItem[0].Value);
            Assert.True(thirdItem[0].Bold);
            Assert.True(thirdItem[0].Italic);
            Assert.True(thirdItem[0].Underline);
        }

        [Fact]
        public void GivenNoTopLevelElements_RendersItInParagraph()
        {
            const string html = "Meow<b>Woof<u>Quack<i>Moo</i></u></b>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Single(_builder.AddedParagraphs);
            var textElements = _builder.AddedParagraphs[0];
            Assert.Equal("MeowWoofQuackMoo", GetParagraphText(textElements));
            Assert.True(textElements[1].Bold);
            Assert.True(textElements[2].Bold);
            Assert.True(textElements[2].Underline);
            Assert.True(textElements[3].Bold);
            Assert.True(textElements[3].Underline);
            Assert.True(textElements[3].Italic);
        }

        [Fact]
        public void GivenStartOfHtmlDoesntContainTopLevelElement_RendersItInAParagraph()
        {
            const string html = "Meow<b>Woof<u>Quack<i>Moo</i></u></b><p>Second paragraph</p>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Equal(2, _builder.AddedParagraphs.Count);
            Assert.Equal("MeowWoofQuackMoo", GetParagraphText(_builder.AddedParagraphs[0]));
        }

        [Fact]
        public void GivenTextNotContainedWithinParagraphMidText_RendersItInAParagraph()
        {
            const string html = "<p>Opening paragraph</p>Meow<b>Woof<u>Quack<i>Moo</i></u></b><p>Second paragraph</p>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Equal(3, _builder.AddedParagraphs.Count);
            Assert.Equal("MeowWoofQuackMoo", GetParagraphText(_builder.AddedParagraphs[1]));
        }

        [Fact]
        public void GivenDivsAsParagraphs_RenderAsParagraphs()
        {
            const string html = "<div>Meow</div><div>Woof</div>";
            HtmlToDocument.Convert(_builder, html);

            Assert.Equal(2, _builder.AddedParagraphs.Count);
            Assert.Equal("Meow", GetParagraphText(_builder.AddedParagraphs[0]));
            Assert.Equal("Woof", GetParagraphText(_builder.AddedParagraphs[1]));
        }

        [Fact]
        public void GivenDivsWithNestedElements_RenderNestedElements()
        {
            const string html = "<div>meow</div><div><ul><li>woof</li></ul></div>";
            HtmlToDocument.Convert(_builder, html);
            Assert.Single(_builder.AddedParagraphs);
            Assert.Single(_builder.AddedLists);
            Assert.Single(_builder.AddedLists[0].AddedItems);
            Assert.Equal("woof", _builder.AddedLists[0].AddedItems[0][0].Value);
        }
        
        [Fact]
        public void GivenSpanWithAttributesAroundText_RenderParagraphWithoutThem()
        {
            const string html = "<div><span style='color:#ff69b4'>meow</span></div>";
            HtmlToDocument.Convert(_builder, html);
            Assert.Single(_builder.AddedParagraphs);
            Assert.Equal("meow", GetParagraphText(_builder.AddedParagraphs[0]));
        }
    }

    public class ParagraphBuilderFake : IParagraphBuilder
    {
        public List<TextElement> AddedItems { get; }

        public ParagraphBuilderFake()
        {
            AddedItems = new List<TextElement>();
        }

        public void AddText(string text)
        {
            AddedItems.Add(new TextElement(text));
        }

        public void AddText(TextElement textElement)
        {
            AddedItems.Add(textElement);
        }

        public void AddText(TextElement[] text)
        {
            foreach (var t in text)
            {
                AddedItems.Add(t);
            }
        }

        public void AddNewLine()
        {
            AddedItems.Add(new TextElement("\n"));
        }

        public void Justify(ParagraphJustification paragraphJustification)
        {
            throw new NotImplementedException();
        }
    }

    public class ListBuilderFake : IListBuilder
    {
        public List<List<TextElement>> AddedItems { get; }

        public ListBuilderFake()
        {
            AddedItems = new List<List<TextElement>>();
        }

        public void AddItem(TextElement item)
        {
            AddedItems.Add(new List<TextElement> {item});
        }

        public void AddItem(string item)
        {
            AddedItems.Add(new List<TextElement> {new TextElement(item)});
        }

        public void AddItem(TextElement[] elements)
        {
            AddedItems.Add(elements.ToList());
        }

        public void AddItem(Action<IParagraphBuilder> action)
        {
            var builder = new ParagraphBuilderFake();
            action(builder);
            AddedItems.Add(builder.AddedItems);
        }
    }

    public class ListFake
    {
        public string Type { get; set; }
        public List<List<TextElement>> AddedItems { get; set; }
    }

    public class DocumentBuilderFake : IDocumentBuilder
    {
        public List<List<TextElement>> AddedParagraphs { get; }
        public List<ListFake> AddedLists { get; }

        public DocumentBuilderFake()
        {
            AddedParagraphs = new List<List<TextElement>>();
            AddedLists = new List<ListFake>();
        }

        public void AddTable(Action<ITableBuilder> action)
        {
            throw new NotImplementedException();
        }

        public void AddTable(IEnumerable<TextElement[]> rows)
        {
            throw new NotImplementedException();
        }

        public void AddParagraph(Action<IParagraphBuilder> action)
        {
            var builder = new ParagraphBuilderFake();
            action(builder);
            AddedParagraphs.Add(builder.AddedItems);
        }

        public void AddParagraph(string text)
        {
            throw new NotImplementedException();
        }

        public void AddHeading(Action<IHeadingBuilder> action)
        {
            throw new NotImplementedException();
        }

        public void AddNumberedList(Action<IListBuilder> action)
        {
            var listFake = new ListFake {Type = "number"};
            var builder = new ListBuilderFake();
            action(builder);
            listFake.AddedItems = builder.AddedItems;
            AddedLists.Add(listFake);
        }

        public void AddBulletedList(Action<IListBuilder> action)
        {
            var listFake = new ListFake {Type = "bullet"};
            var builder = new ListBuilderFake();
            action(builder);
            listFake.AddedItems = builder.AddedItems;
            AddedLists.Add(listFake);
        }

        public void ReplacePlaceholderWithContent(string placeholderText, Action<DocumentBodyBuilder> action)
        {
            throw new NotImplementedException();
        }

        public void AddHeader(Action<IHeaderBuilder> action)
        {
            throw new NotImplementedException();
        }

        public void AddFooter(Action<IFooterBuilder> action)
        {
            throw new NotImplementedException();
        }

        public byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}