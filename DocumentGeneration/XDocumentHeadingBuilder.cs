using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentGeneration
{
    public static class XDocumentHeadingBuilder
    {
        public enum HeadingLevelOptions
        {
            Heading1,
            Heading2,
            Heading3
        }

        public static void AddHeadingToElement(OpenXmlElement parentElement, string text,
            HeadingLevelOptions headingLevel)
        {
            var run = new Run(new Text(text)) {RunProperties = new RunProperties()};
            var runProperties = run.RunProperties;
            var headingLevelValues = new Dictionary<HeadingLevelOptions, string>
            {
                {HeadingLevelOptions.Heading1, "60"},
                {HeadingLevelOptions.Heading2, "40"},
                {HeadingLevelOptions.Heading3, "30"}
            };
            runProperties.FontSize = new FontSize {Val = headingLevelValues[headingLevel]};
            var para = new Paragraph(run);
                
            parentElement.AppendChild(para);
        }
    }
}