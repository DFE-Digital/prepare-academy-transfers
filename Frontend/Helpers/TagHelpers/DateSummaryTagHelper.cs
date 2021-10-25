using Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Frontend.Helpers.TagHelpers
{
    [HtmlTargetElement("datesummary")]
    public class DateSummaryTagHelper : TagHelper
    {
        public string Value { get; set; }
        public bool? HasDate { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            if (string.IsNullOrEmpty(Value) && HasDate == null)
            {
                output.Attributes.SetAttribute("class", "dfe-empty-tag");
                output.TagName = "span";
                output.Content.SetContent("Empty");
            }
            else
            {
                output.Content.SetContent(DatesHelper.FormatDateString(Value, HasDate));
            }

            base.Process(context, output);
        }
    }
}