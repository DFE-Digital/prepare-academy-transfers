using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Frontend.Helpers.TagHelpers
{
    [HtmlTargetElement("gds-key-value", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GdsKeyValueTagHelper : TagHelper
    {
        public string Key { get; set; }
        public string Value { get; set; }
        
        private readonly HtmlEncoder _htmlEncoder;

        public GdsKeyValueTagHelper(HtmlEncoder htmlEncoder)
        {
            _htmlEncoder = htmlEncoder;
        }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.AddClass("govuk-summary-list__row", HtmlEncoder.Default);

            var dt = new TagBuilder("dt");
            dt.AddCssClass("govuk-summary-list__key");
            dt.InnerHtml.SetContent(Key);
            
            var dd = new TagBuilder("dd");
            dd.AddCssClass("govuk-summary-list__value");
            dd.InnerHtml.SetHtmlContent(RenderInnerTagHelper());

            output.Content.AppendHtml(dt.RenderStartTag());
            output.Content.AppendHtml(dt.RenderBody());
            output.Content.AppendHtml(dt.RenderEndTag());
            
            output.Content.AppendHtml(dd.RenderStartTag());
            output.Content.AppendHtml(dd.RenderBody());
            output.Content.AppendHtml(dd.RenderEndTag());
        }
        
        private string RenderInnerTagHelper()
        {
            DisplayNoDataForEmptyStringTagHelper innerTagHelper = new DisplayNoDataForEmptyStringTagHelper();
            // var attributes = new TagHelperAttributeList {{nameof(DisplayNoDataForEmptyStringTagHelper.Value), Value}};
            innerTagHelper.Value = Value;
            // Create a TagHelperOutput instance
            TagHelperOutput innerOutput = new TagHelperOutput(
                "span",
                new TagHelperAttributeList(),
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
            innerTagHelper.Process(innerContext, innerOutput);
            
            // Render and return the tag helper attributes and content
            return RenderTagHelperOutput(innerOutput);
        }

        /*
         * Helper Method to gather the html content
         */
        private string RenderTagHelperOutput(TagHelperOutput output)
        {
            using (var writer = new StringWriter())
            {
                output.WriteTo(writer, _htmlEncoder);
                return writer.ToString();
            }
        }
    }
}



// <div class="govuk-summary-list__row">
//     <dt class="govuk-summary-list__key">
//     </dt>
//     <dd class="govuk-summary-list__value">
//     <displaynodataforemptystring value="@field.Value"></displaynodataforemptystring>
//     </dd>
//     </div>