using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers.TagHelpers
{
    [HtmlTargetElement("displaynodataforemptystring")]
    public class DisplayNoDataForEmptyStringTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            var childContent = await output.GetChildContentAsync();
            if (string.IsNullOrWhiteSpace(childContent?.GetContent()))
            {
                output.Content.SetContent("No data");
            }

            await base.ProcessAsync(context, output);
        }
    }
}