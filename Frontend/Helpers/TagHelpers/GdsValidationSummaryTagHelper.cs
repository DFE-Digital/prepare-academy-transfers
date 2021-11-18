using System;
using System.Linq;
using System.Text;
using Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Frontend.Helpers.TagHelpers
{
    [HtmlTargetElement("div", Attributes = MyValidationForAttributeName)]
    public class GdsValidationSummaryTagHelper : TagHelper
    {
        private const string MyValidationForAttributeName = "asp-gds-validation-summary";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            var viewData = ViewContext.ViewData;
            if (!ViewContext.ClientValidationEnabled && viewData.ModelState.IsValid)
            {
                output.SuppressOutput();
                return;
            }

            var modelStateErrors = ViewContext.ViewData.ModelState.Where(ms => ms.Value.Errors.Any())
                .Select(a => new { a.Key, a.Value.Errors }).ToList();
            
            var sb = new StringBuilder();
            sb.Append("<div class='govuk-grid-row'>");
            sb.Append("<div class='govuk-grid-column-full'>");
            sb.Append("<div class='govuk-error-summary' aria-labelledby='error-summary-title' role='alert' tabindex='-1' data-module='govuk-error-summary' data-ga-event-form='error' data-qa='error'>");
            sb.Append("<h2 class='govuk-error-summary__title' id='error-summary-title' data-qa='error__heading'>");
            sb.Append("There is a problem");
            sb.Append("</h2>");
            sb.Append("<div class='govuk-error-summary__body'>");
            sb.Append("<ul class='govuk-list govuk-error-summary__list'>");

            foreach (var p in modelStateErrors)
            {
                foreach (var e in p.Errors)
                {
                    sb.Append("<li>");
                    sb.Append($"<a href='#{p.Key.ToHtmlName()}' data-qa='error_text'>");
                    sb.Append(e.ErrorMessage);
                    sb.Append("</a>");
                    sb.Append("</li>");
                }
            }

            sb.Append("</ul>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
    
   
}