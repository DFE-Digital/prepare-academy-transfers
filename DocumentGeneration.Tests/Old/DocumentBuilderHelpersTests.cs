using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentGeneration.Helpers;
using Xunit;

namespace DocumentGeneration.Tests.Old
{
    public class DocumentBuilderHelpersTests
    {
        public class AddTextToElementTests
        {
            [Fact]
            public void GivenSimpleTest_AddsTextToElement()
            {
                var run = new Run();
                DocumentBuilderHelpers.AddTextToElement(run, "Example text");

                var textElements = run.Descendants<Text>().ToList();

                Assert.Single(textElements);
                Assert.Equal("Example text", textElements[0].InnerText);
            }

            [Fact]
            public void GivenTextWithNewLine_AddsTwoTextElementsSeparatedByBreak()
            {
                var run = new Run();
                DocumentBuilderHelpers.AddTextToElement(run, "Example text\nWith a new line");

                var textElements = run.Descendants<Text>().ToList();
                var descendents = run.Descendants().ToList();

                Assert.Equal(2, textElements.Count);
                Assert.IsType<Break>(descendents[1]);
                Assert.Equal("Example text", textElements[0].InnerText);
                Assert.Equal("With a new line", textElements[1].InnerText);
            }

            [Fact]
            public void GivenTextWithSeveralNewLines_AddsTheCorrectNumberOfTextElementsAndBreaks()
            {
                var run = new Run();
                DocumentBuilderHelpers.AddTextToElement(run, "Example text\nWith a new line\n3\n4\n5");

                var textElements = run.Descendants<Text>().ToList();
                var breakElements = run.Descendants<Break>().ToList();

                Assert.Equal(5, textElements.Count);
                Assert.Equal(4, breakElements.Count);
            }

            [Fact]
            public void GivenTextWithCarriageReturn_AddsTwoTextElementsSeparatedByBreak()
            {
                var run = new Run();
                DocumentBuilderHelpers.AddTextToElement(run, "Example text\rWith a new line");

                var textElements = run.Descendants<Text>().ToList();
                var descendents = run.Descendants().ToList();

                Assert.Equal(2, textElements.Count);
                Assert.IsType<Break>(descendents[1]);
                Assert.Equal("Example text", textElements[0].InnerText);
                Assert.Equal("With a new line", textElements[1].InnerText);
            }

            [Fact]
            public void GivenTextWithNewLineCarriageReturn_AddsTwoTextElementsSeparatedByBreak()
            {
                var run = new Run();
                DocumentBuilderHelpers.AddTextToElement(run, "Example text\r\nWith a new line");

                var textElements = run.Descendants<Text>().ToList();
                var descendents = run.Descendants().ToList();

                Assert.Equal(2, textElements.Count);
                Assert.IsType<Break>(descendents[1]);
                Assert.Equal("Example text", textElements[0].InnerText);
                Assert.Equal("With a new line", textElements[1].InnerText);
            }
        }
    }
}