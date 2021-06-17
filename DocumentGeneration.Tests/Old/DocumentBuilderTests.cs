using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Xunit;

namespace DocumentGeneration.Tests.Old
{
    public class DocumentBuilderTests
    {
        [Fact]
        public void GivenTextForParagraph_CreatesADocumentWithAParagraph()
        {
            var documentBody = GenerateDocument(builder => builder.AddParagraph("This is an example paragraph"));
            var textDescendents = documentBody.Descendants<Text>().ToList();

            Assert.Single(textDescendents);
            Assert.Equal("This is an example paragraph", textDescendents.First().InnerText);
        }

        [Fact]
        public void GivenMultipleParagraphsAdded_CreatesADocumentWithMultipleParagraphs()
        {
            var documentBody = GenerateDocument(builder =>
            {
                builder.AddParagraph("Paragraph one");
                builder.AddParagraph("Paragraph two");
                builder.AddParagraph("Paragraph three");
            });

            var textDescendents = documentBody.Descendants<Text>().ToList();

            Assert.Equal(3, textDescendents.Count);
            Assert.Equal("Paragraph one", textDescendents[0].InnerText);
            Assert.Equal("Paragraph two", textDescendents[1].InnerText);
            Assert.Equal("Paragraph three", textDescendents[2].InnerText);
        }

        [Fact]
        public void GivenHeadingAdded_CreatesADocumentWithTheTextAddedCorrectly()
        {
            var documentBody = GenerateDocument(builder =>
            {
                builder.AddHeading("Heading 1", XDocumentHeadingBuilder.HeadingLevelOptions.Heading1);
                builder.AddHeading("Heading 2", XDocumentHeadingBuilder.HeadingLevelOptions.Heading2);
                builder.AddHeading("Heading 3", XDocumentHeadingBuilder.HeadingLevelOptions.Heading3);
            });

            var textDescendents = documentBody.Descendants<Text>().ToList();
            Assert.Equal(3, textDescendents.Count);
            Assert.Equal("Heading 1", textDescendents[0].InnerText);
            Assert.Equal("Heading 2", textDescendents[1].InnerText);
            Assert.Equal("Heading 3", textDescendents[2].InnerText);
        }

        [Fact]
        public void GivenTableAdded_CreatesADocumentWithATable()
        {
            var documentBody = GenerateDocument(builder =>
            {
                builder.AddTable(new List<List<string>> {new List<string> {"Meow"}});
            });

            var tableDescendents = documentBody.Descendants<Table>().ToList();
            Assert.Single(tableDescendents);
            Assert.Equal("Meow", tableDescendents[0].InnerText);
        }

        [Fact]
        public void GivenMultipleTablesAdded_CreatesADocumentWithMultipleTables()
        {
            var documentBody = GenerateDocument(builder =>
            {
                builder.AddTable(new List<List<string>> {new List<string> {"Meow"}});
                builder.AddTable(new List<List<string>> {new List<string> {"Woof"}});
                builder.AddTable(new List<List<string>> {new List<string> {"Quack"}});
            });

            var tableDescendents = documentBody.Descendants<Table>().ToList();
            Assert.Equal(3, tableDescendents.Count);
            Assert.Equal("Meow", tableDescendents[0].InnerText);
            Assert.Equal("Woof", tableDescendents[1].InnerText);
            Assert.Equal("Quack", tableDescendents[2].InnerText);
        }

        #region Helpers

        private static Body GenerateDocument(Action<XDocumentBuilder> documentFunction)
        {
            MemoryStream memoryStream;
            Body documentBody;

            using (memoryStream = new MemoryStream())
            {
                var builder = new XDocumentBuilder(memoryStream);

                documentFunction(builder);

                builder.Build();
                using (var doc = WordprocessingDocument.Open(memoryStream, false))
                {
                    documentBody = doc.MainDocumentPart.Document.Body;
                }
            }

            return documentBody;
        }

        #endregion
    }
}