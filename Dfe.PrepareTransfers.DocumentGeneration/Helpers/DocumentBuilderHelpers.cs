using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Dfe.PrepareTransfers.DocumentGeneration.Helpers
{
    public static class DocumentBuilderHelpers
    {
        public static void AddTextToElement(OpenXmlElement element, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                element.AppendChild(new Text(""));
            }
            else
            {
                var splitText = text.Split(new[] {"\r\n", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in splitText)
                {
                    var textElement = new Text(line)
                    {
                        Space = new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve)
                    };
                    
                    element.AppendChild(textElement);
                    if (line != splitText.Last())
                    {
                        element.AppendChild(new Break());
                    }
                }
            }
        }
    }
}