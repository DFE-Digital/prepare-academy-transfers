using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentGeneration
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
                    element.AppendChild(new Text(line));
                    if (line != splitText.Last())
                    {
                        element.AppendChild(new Break());
                    }
                }
            }
        }
    }
}