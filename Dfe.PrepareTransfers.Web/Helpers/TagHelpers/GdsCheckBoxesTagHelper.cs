using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers.TagHelpers
{
    [HtmlTargetElement("gds-checkboxes", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GdsCheckBoxesTagHelper : TagHelper
    {
        public IList<CheckboxViewModel> Checkboxes { get; set; }

        public bool WithoutContainer { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (WithoutContainer)
            {
                output.TagName = null;
            }
            else
            {
                output.TagName = "div";
                output.AddClass("govuk-checkboxes", HtmlEncoder.Default);
                output.Attributes.Add("data-module", "govuk-checkboxes");
            }

            for (int i = 0; i < Checkboxes.Count; i++)
            {
                var checkBox = Checkboxes[i];
                var nameOrValue = (i == 0 ? checkBox.Name.ToHtmlName() : checkBox.Value);

                var div = new TagBuilder("div");
                div.AddCssClass("govuk-checkboxes__item");
                var input = new TagBuilder("input");
                input.AddCssClass("govuk-checkboxes__input");
                input.MergeAttribute("type", "checkbox");
                input.MergeAttribute("id", nameOrValue);
                input.MergeAttribute("name", checkBox.Name);
                input.MergeAttribute("value", checkBox.Value);
                if (checkBox.Checked)
                {
                    input.MergeAttribute("checked", null);
                }

                var label = new TagBuilder("label");
                label.AddCssClass("govuk-label govuk-checkboxes__label");
                label.MergeAttribute("for", nameOrValue);
                label.InnerHtml.AppendHtml(checkBox.DisplayName);

                output.Content.AppendHtml(div.RenderStartTag());
                output.Content.AppendHtml(input.RenderStartTag());
                output.Content.AppendHtml(label.RenderStartTag());
                output.Content.AppendHtml(label.InnerHtml);
                output.Content.AppendHtml(label.RenderEndTag());
                output.Content.AppendHtml(div.RenderEndTag());
            }
        }
    }
}
