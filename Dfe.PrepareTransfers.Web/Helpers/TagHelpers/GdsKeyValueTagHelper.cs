using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers.TagHelpers
{
    [HtmlTargetElement("gds-key-value", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GdsKeyValueTagHelper : TagHelper
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool ShowAction { get; set; }
        public string ActionUrl { get; set; }
        public string ActionText { get; set; }

        private readonly HtmlEncoder _htmlEncoder;

        public GdsKeyValueTagHelper(HtmlEncoder htmlEncoder)
        {
            _htmlEncoder = htmlEncoder;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.AddClass("govuk-summary-list__row", _htmlEncoder);
            if (!ShowAction)
            {
                output.AddClass("govuk-summary-list__row--no-actions", _htmlEncoder);
            }

            var dt = new TagBuilder("dt");
            dt.AddCssClass("govuk-summary-list__key");
            dt.InnerHtml.SetContent(Key);

            var dd = new TagBuilder("dd");
            dd.AddCssClass("dfe-summary-list__value--width-50");
            dd.AddCssClass("govuk-summary-list__value");

            dd.InnerHtml.SetHtmlContent(string.IsNullOrWhiteSpace(Value) ? "No data" : Value);

            output.Content.AppendHtml(dt.RenderStartTag());
            output.Content.AppendHtml(dt.RenderBody());
            output.Content.AppendHtml(dt.RenderEndTag());

            output.Content.AppendHtml(dd.RenderStartTag());
            output.Content.AppendHtml(dd.RenderBody());
            output.Content.AppendHtml(dd.RenderEndTag());

            if (ShowAction && !string.IsNullOrWhiteSpace(ActionUrl) && !string.IsNullOrWhiteSpace(ActionText))
            {
                var actionDiv = new TagBuilder("div");
                actionDiv.AddCssClass("govuk-summary-list__actions");

                var actionLink = new TagBuilder("a");
                actionLink.AddCssClass("govuk-link");
                actionLink.Attributes["href"] = ActionUrl;
                actionLink.InnerHtml.SetContent(ActionText);

                actionDiv.InnerHtml.AppendHtml(actionLink);
                output.Content.AppendHtml(actionDiv);
            }
        }
    }
}
