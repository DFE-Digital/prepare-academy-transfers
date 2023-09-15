using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers.TagHelpers
{
    [HtmlTargetElement("stringsummary")]
    public class StringSummaryTagHelper : TagHelper
    {
        public string Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            if (!string.IsNullOrEmpty(Value))
            {
                output.Content.SetContent(Value);
            }
            else
            {
                output.Attributes.SetAttribute("class", "dfe-empty-tag");
                output.TagName = "span";
                output.Content.SetContent("Empty");
            }

            base.Process(context, output);
        }
    }
}