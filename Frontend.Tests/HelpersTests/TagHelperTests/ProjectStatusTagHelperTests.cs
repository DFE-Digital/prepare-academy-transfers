using Data.Models;
using Frontend.Helpers.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Tests.HelpersTests.TagHelperTests
{
    public class ProjectStatusTagHelperTests
    {
        [Theory]
        [InlineData(ProjectStatuses.NotStarted, "NOT STARTED", "govuk-tag--red")]
        [InlineData(ProjectStatuses.InProgress, "IN PROGRESS", null)]
        [InlineData(ProjectStatuses.Completed, "COMPLETED", "govuk-tag--green")]
        [InlineData(ProjectStatuses.Empty, "", null)]
        public void GivenNotStartedStatus_ReturnsRedNotStartedTag(ProjectStatuses projectStatus, string expectedStatusText, string expectedCssClass)
        {
            // Arrange
            var projectStatusTagHelper = new ProjectStatusTagHelper
            {
                Status = projectStatus
            };
            var tagHelperContext = new TagHelperContext(
                            new TagHelperAttributeList(),
                            new Dictionary<object, object>(),
                            Guid.NewGuid().ToString("N"));
            var tagHelperOutput = new TagHelperOutput("projectstatus",
                new TagHelperAttributeList(),
                (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        tagHelperContent.SetHtmlContent(string.Empty);
                        return Task.FromResult<TagHelperContent>(tagHelperContent);
                    });

            // Act
            projectStatusTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.Equal("strong", tagHelperOutput.TagName);
            Assert.Equal(expectedStatusText, tagHelperOutput.Content.GetContent());
            Assert.Equal($"govuk-tag {expectedCssClass} moj-task-list__tag", tagHelperOutput.Attributes.Single().Value);
        }
    }
}
