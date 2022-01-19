using Data.Models;
using Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Frontend.Helpers.TagHelpers
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
            
            output.Attributes.SetAttribute("class", $"govuk-tag {tagColourClass} moj-task-list__tag");
            output.Attributes.SetAttribute("data-test", context.AllAttributes["id"].Value.ToString());
            output.TagName = "strong";
            output.Content.SetContent(EnumHelpers<ProjectStatuses>.GetDisplayValue(Status));
            base.Process(context, output);
        }
    }
}
