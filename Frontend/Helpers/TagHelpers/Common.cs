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
            TagHelperOutput innerOutput = new TagHelperOutput(
                tagName,
                tagHelperAttributeList,
                (useCachedResult, encoder) =>
                    Task.Run<TagHelperContent>(() => new DefaultTagHelperContent())
            )
            {
                TagMode = TagMode.StartTagAndEndTag
            };
            
            TagHelperContext innerContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString()
            );
            
            tagHelper.Process(innerContext, innerOutput);
            
            using var writer = new StringWriter();
            innerOutput.WriteTo(writer, htmlEncoder);
            return writer.ToString();
        }
    }
}