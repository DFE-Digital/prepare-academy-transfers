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
        public static async Task<string> RenderTagHelper(TagHelper tagHelper, string tagName,
            TagHelperAttributeList tagHelperAttributeList, HtmlEncoder htmlEncoder, Func<bool,HtmlEncoder,Task<TagHelperContent>> getChildContent)
        {
            TagHelperOutput innerOutput = new TagHelperOutput(
                tagName,
                tagHelperAttributeList,
                getChildContent
            )
            {
                TagMode = TagMode.StartTagAndEndTag
            };
            
            TagHelperContext innerContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString()
            );
            
            await tagHelper.ProcessAsync(innerContext, innerOutput);
            
            using var writer = new StringWriter();
            innerOutput.WriteTo(writer, htmlEncoder);
            return writer.ToString();
        }
    }
}