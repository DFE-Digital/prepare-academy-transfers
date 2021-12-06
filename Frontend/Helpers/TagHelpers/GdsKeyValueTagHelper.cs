using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
        
        public bool ShowAction { get; set; }
        
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

            var dt = new TagBuilder("dt");
            dt.AddCssClass("govuk-summary-list__key");
            dt.InnerHtml.SetContent(Key);
            
            var dd = new TagBuilder("dd");
            dd.AddCssClass("dfe-summary-list__value--width-50");
            dd.AddCssClass("govuk-summary-list__value");
            var noDataTagHelper = new DisplayNoDataForEmptyStringTagHelper
            {
                Value = Value
            };
            var noDataTagHelperResult = Common.RenderTagHelper(noDataTagHelper, "span", new TagHelperAttributeList(), _htmlEncoder);

            dd.InnerHtml.SetHtmlContent(WebUtility.HtmlDecode(noDataTagHelperResult));
            
            output.Content.AppendHtml(dt.RenderStartTag());
            output.Content.AppendHtml(dt.RenderBody());
            output.Content.AppendHtml(dt.RenderEndTag());
            
            output.Content.AppendHtml(dd.RenderStartTag());
            output.Content.AppendHtml(dd.RenderBody());
            output.Content.AppendHtml(dd.RenderEndTag());

            if (ShowAction)
            {
                var ddAction = new TagBuilder("dd");
                ddAction.AddCssClass("govuk-summary-list__actions");
                output.Content.AppendHtml(ddAction.RenderStartTag());
                output.Content.AppendHtml(ddAction.RenderEndTag());
            }
        }
    }
}