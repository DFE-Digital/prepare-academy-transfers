using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Xunit;

namespace DocumentGeneration.Tests.Old
{
    public class DocumentHeadingBuilderTests
    {
        [Fact]
        public void GivenHeadingOne_CreatesARunWithTheExpectedRunProperties()
        {
            var body = new Body();
            XDocumentHeadingBuilder.AddHeadingToElement(body, "Heading 1",
                XDocumentHeadingBuilder.HeadingLevelOptions.Heading1);

            var createdRuns = body.Descendants<Run>().ToList();
            Assert.Equal("60", createdRuns[0].RunProperties.FontSize.Val);
            Assert.Equal("Heading 1", createdRuns[0].InnerText);
        }

        [Fact]
        public void GivenHeadingTwo_CreatesARunWithTheExpectedRunProperties()
        {
            var body = new Body();
            XDocumentHeadingBuilder.AddHeadingToElement(body, "Heading 2",
                XDocumentHeadingBuilder.HeadingLevelOptions.Heading2);

            var createdRuns = body.Descendants<Run>().ToList();
            Assert.Equal("40", createdRuns[0].RunProperties.FontSize.Val);
            Assert.Equal("Heading 2", createdRuns[0].InnerText);
        }

        [Fact]
        public void GivenHeadingThree_CreatesARunWithTheExpectedRunProperties()
        {
            var body = new Body();
            XDocumentHeadingBuilder.AddHeadingToElement(body, "Heading 3",
                XDocumentHeadingBuilder.HeadingLevelOptions.Heading3);

            var createdRuns = body.Descendants<Run>().ToList();
            Assert.Equal("30", createdRuns[0].RunProperties.FontSize.Val);
            Assert.Equal("Heading 3", createdRuns[0].InnerText);
        }
    }
}