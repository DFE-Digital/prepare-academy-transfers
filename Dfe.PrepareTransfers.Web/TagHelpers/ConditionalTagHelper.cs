using Microsoft.AspNetCore.Razor.TagHelpers;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Dfe.PrepareTransfers.Web.TagHelpers;

[HtmlTargetElement(Attributes = "if")]
public class ConditionalTagHelper : TagHelper
{
    [HtmlAttributeName("if")]
    public bool Conditional { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Conditional is false) output.SuppressOutput();
        else base.Process(context, output);
    }
}
