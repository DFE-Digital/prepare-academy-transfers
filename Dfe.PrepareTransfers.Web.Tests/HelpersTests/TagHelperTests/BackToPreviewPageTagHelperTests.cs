using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Helpers.TagHelpers;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.HelpersTests.TagHelperTests
{
    public class BackToPreviewPageTagHelperTests
    {
        [Fact]
        public void GivenBackToPreview_RendersPreviewPageLink()
        {
            var linkGenerator = new Mock<LinkGenerator>();
            var projectStatusTagHelper = new BackToPreviewPageTagHelper(linkGenerator.Object)
            {
                ReturnToPreview = true,
                Urn = "1000"
            };
            
            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var tagHelperOutput = new TagHelperOutput("backtopreview",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    var helperContent = tagHelperContent.SetHtmlContent("Meow");
                    return Task.FromResult(helperContent);
                });

            projectStatusTagHelper.Process(tagHelperContext, tagHelperOutput);

            Assert.Equal("a", tagHelperOutput.TagName);
            Assert.Equal(Links.HeadteacherBoard.Preview.BackText, tagHelperOutput.Content.GetContent());
            Assert.True(tagHelperOutput.Attributes.ContainsName("href"));
        }

        [Fact]
        public void GivenNotBackToPreview_RendersExistingContent()
        {
            var linkGenerator = new Mock<LinkGenerator>();
            var projectStatusTagHelper = new BackToPreviewPageTagHelper(linkGenerator.Object)
            {
                ReturnToPreview = false,
                Urn = "1000"
            };
            
            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var tagHelperOutput = new TagHelperOutput("backtopreview",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    var helperContent = tagHelperContent.SetHtmlContent("<a>Content</a>");
                    return Task.FromResult(helperContent);
                });

            projectStatusTagHelper.Process(tagHelperContext, tagHelperOutput);
            
            Assert.Null(tagHelperOutput.TagName);
            Assert.Equal("<a>Content</a>", tagHelperOutput.Content.GetContent());
        }
    }
}