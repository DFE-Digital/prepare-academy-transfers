using Data.Models;
using Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.PrepareTransfers.Web.Helpers.TagHelpers
{
    [HtmlTargetElement("projectstatus")]
    public class ProjectStatusTagHelper : TagHelper
    {
        public ProjectStatuses Status { get; set; } = ProjectStatuses.NotStarted;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string tagColourClass = string.Empty;
            switch (Status)
            {
                case ProjectStatuses.NotStarted:
                    tagColourClass = "govuk-tag--grey";
                    break;
                case ProjectStatuses.InProgress:
                    tagColourClass = "govuk-tag--blue";
                    break;
            }

            var section = context.AllAttributes["id"].Value;

            output.Attributes.SetAttribute("class", $"govuk-tag {tagColourClass} moj-task-list__tag");
            output.Attributes.SetAttribute("data-test", section);
            output.Attributes.SetAttribute($"data-google-analytics-project-status", $"{section},{Status == ProjectStatuses.Completed}".ToLower());
            output.TagName = "strong";
            output.Content.SetContent(EnumHelpers<ProjectStatuses>.GetDisplayValue(Status));
            base.Process(context, output);
        }
    }
}
