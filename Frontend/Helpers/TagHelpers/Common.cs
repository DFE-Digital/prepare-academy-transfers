using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Frontend.Helpers.TagHelpers
{
    public static class Common
    {
        public static string RenderTagHelper(TagHelper tagHelper, string tagName,
            TagHelperAttributeList tagHelperAttributeList, HtmlEncoder htmlEncoder)
        {
            // Create a TagHelperOutput instance
            TagHelperOutput innerOutput = new TagHelperOutput(
                tagName,
                tagHelperAttributeList,
                (useCachedResult, encoder) =>
                    Task.Run<TagHelperContent>(() => new DefaultTagHelperContent())
            )
            {
                TagMode = TagMode.StartTagAndEndTag
            };
            // Create a TagHelperContext instance
            TagHelperContext innerContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString()
            );

            // Process the InnerTagHelper instance 
            tagHelper.Process(innerContext, innerOutput);

            // Render and return the tag helper attributes and content
            using var writer = new StringWriter();
            innerOutput.WriteTo(writer, htmlEncoder);
            return writer.ToString();
        }
    }
}