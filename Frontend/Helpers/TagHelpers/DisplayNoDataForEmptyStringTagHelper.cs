using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Frontend.Helpers.TagHelpers
{
    [HtmlTargetElement("displaynodataforemptystring")]
    public class DisplayNoDataForEmptyStringTagHelper : TagHelper
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
                output.TagName = "span";
                output.Content.SetContent("No data");
            }

            base.Process(context, output);
        }
    }
}