using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.PrepareTransfers.TagHelpers;

public class SubMenuLinkTagHelper : AnchorTagHelper
{
   private const string PAGE = "page";
   public SubMenuLinkTagHelper(IHtmlGenerator generator) : base(generator) { }

   public override void Process(TagHelperContext context, TagHelperOutput output)
   {
      string page = ViewContext.RouteData.Values[PAGE]!.ToString();
      if (page == Page)
      {
         output.Attributes.SetAttribute("aria-current", PAGE);
      }

      output.TagName = "a";

      base.Process(context, output);
   }
}
