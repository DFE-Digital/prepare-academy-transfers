using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentGeneration.Elements;
using DocumentGeneration.Interfaces;
using DocumentGeneration.Interfaces.Parents;

namespace DocumentGeneration.Helpers
{
    public static class HtmlToDocument
    {
        private const string ElementPattern = @"(</?.*?>)";
        private const string TopLevelElementOpenPattern = @"<(?:ol|ul|p|div)>";

        public static void Convert(IDocumentBuilder builder, string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            var elements = SplitHtmlIntoElements(html);
            var res = SplitHtmlIntoTopLevelElements(elements);
            BuildDocumentFromTopLevelElements(builder, res);
        }

        private static void BuildDocumentFromTopLevelElements(IDocumentBuilder builder, List<List<string>> res)
        {
            foreach (var elementList in res)
            {
                switch (elementList[0])
                {
                    case "<p>":
                        builder.AddParagraph(pBuilder =>
                        {
                            BuildParagraphFromElements(pBuilder, elementList.Skip(1).ToList());
                        });
                        break;
                    case "<div>":
                        var elements = elementList.Skip(1).ToList();
                        var topLevelElements = SplitHtmlIntoTopLevelElements(elements);
                        BuildDocumentFromTopLevelElements(builder, topLevelElements);
                        break;
                    case "<ul>":
                        builder.AddBulletedList(lBuilder =>
                        {
                            BuildListFromElements(lBuilder, elementList.Skip(1).ToList());
                        });
                        break;
                    case "<ol>":
                        builder.AddNumberedList(lBuilder =>
                        {
                            BuildListFromElements(lBuilder, elementList.Skip(1).ToList());
                        });
                        break;
                    default:
                        builder.AddParagraph(pBuilder => { BuildParagraphFromElements(pBuilder, elementList); });
                        break;
                }
            }
        }

        private static List<List<string>> SplitHtmlIntoTopLevelElements(List<string> elementsToGroup)
        {
            var res = new List<List<string>>();

            if (!elementsToGroup.Any(element => Regex.IsMatch(element, TopLevelElementOpenPattern)))
            {
                return res.Append(elementsToGroup.Prepend("<p>").ToList()).ToList();
            }

            if (!Regex.IsMatch(elementsToGroup[0], TopLevelElementOpenPattern))
            {
                var nextElements = elementsToGroup
                    .TakeWhile(element => !Regex.IsMatch(element, TopLevelElementOpenPattern))
                    .Prepend("<p>").ToList();
                res.Add(nextElements);

                elementsToGroup = elementsToGroup
                    .SkipWhile(element => !Regex.IsMatch(element, TopLevelElementOpenPattern))
                    .ToList();
            }

            while (elementsToGroup.Count > 0)
            {
                List<string> nextElement;
                (nextElement, elementsToGroup) = ParseNextTopLevelElement(elementsToGroup);
                res.Add(nextElement);
            }

            return res;
        }

        private static List<string> SplitHtmlIntoElements(string html)
        {
            return Regex.Split(html, ElementPattern).Where(element => !string.IsNullOrEmpty(element)).ToList();
        }

        private static void BuildListFromElements(IListBuilder lBuilder, List<string> elements)
        {
            while (elements.Count > 0)
            {
                elements = GetNextListItem(lBuilder, elements);
            }
        }

        private static List<string> GetNextListItem(IListBuilder listBuilder, List<string> elements)
        {
            var elementsToParse = elements.Skip(1).TakeWhile(element => element != "</li>").ToList();
            var remaining = elements.SkipWhile(element => element != "</li>").Skip(1).ToList();
            listBuilder.AddItem(pBuilder => { BuildParagraphFromElements(pBuilder, elementsToParse); });
            return remaining;
        }

        private static void BuildParagraphFromElements(IParagraphBuilder pBuilder, List<string> elements)
        {
            var tagsEnabled = new List<string>();

            foreach (var element in elements)
            {
                switch (element)
                {
                    case "<b>":
                        tagsEnabled.Add("b");
                        break;
                    case "</b>":
                        tagsEnabled.Remove("b");
                        break;
                    case "<u>":
                        tagsEnabled.Add("u");
                        break;
                    case "</u>":
                        tagsEnabled.Remove("u");
                        break;
                    case "<i>":
                        tagsEnabled.Add("i");
                        break;
                    case "</i>":
                        tagsEnabled.Remove("i");
                        break;
                    case "<br>":
                        pBuilder.AddNewLine();
                        break;
                    default:
                        if (!Regex.IsMatch(element, ElementPattern))
                        {
                            AddTextElementToParagraph(pBuilder, element, tagsEnabled);
                        }

                        break;
                }
            }
        }

        private static void AddTextElementToParagraph(ITextParent pBuilder, string toAdd,
            List<string> tagsEnabled)
        {
            var textElement = TextElementWithFormatting(toAdd, tagsEnabled);
            pBuilder.AddText(textElement);
        }

        private static TextElement TextElementWithFormatting(string toAdd, List<string> tagsEnabled)
        {
            var textElement = new TextElement(toAdd);
            if (tagsEnabled.Contains("b")) textElement.Bold = true;
            if (tagsEnabled.Contains("i")) textElement.Italic = true;
            if (tagsEnabled.Contains("u")) textElement.Underline = true;
            return textElement;
        }

        private static (List<string>, List<string>) ParseNextTopLevelElement(List<string> elements)
        {
            var next = elements[0];
            var remaining = elements.Skip(1).ToList();
            if (!Regex.IsMatch(next, TopLevelElementOpenPattern))
            {
                var nextElements = remaining
                    .TakeWhile(word => !Regex.IsMatch(word, TopLevelElementOpenPattern))
                    .Prepend(next)
                    .ToList();
                var leftover = remaining
                    .SkipWhile(word => !Regex.IsMatch(word, TopLevelElementOpenPattern))
                    .ToList();

                return (nextElements, leftover);
            }

            var closingTag = next.Insert(1, "/");
            var res = remaining.TakeWhile(word => word != closingTag).Prepend(next);
            return (res.ToList(), remaining.SkipWhile(word => word != closingTag).Skip(1).ToList());
        }
    }
}