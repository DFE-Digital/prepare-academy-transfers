using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Frontend.Helpers.TagHelpers;
using Frontend.Models.Forms;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Frontend.Tests.HelpersTests.TagHelperTests
{
    public class GdsKeyValueTagHelperTests
    {
        private readonly TagHelperContext _tagHelperContext;
        private readonly TagHelperOutput _tagHelperOutput;

        public GdsKeyValueTagHelperTests()
        {
            _tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            _tagHelperOutput = new TagHelperOutput("div",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    var helperContent = tagHelperContent.SetHtmlContent("Test");
                    return Task.FromResult(helperContent);
                });
        }
        
        [Fact]
        public void GivenKeyValues_RenderSummaryListRow()
        {
            var tagHelper = new GdsKeyValueTagHelper(HtmlEncoder.Default)
            {
               Key = "My Key",
               Value = "My Value"
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);
            
            var expectedContent =
                "<dt class=\"govuk-summary-list__key\">My Key</dt><dd class=\"govuk-summary-list__value dfe-summary-list__value--width-50\"><span>My Value</span></dd>";
            
            Assert.Equal("div", _tagHelperOutput.TagName);
            Assert.Equal("govuk-summary-list__row", _tagHelperOutput.Attributes["class"].Value);
            Assert.Equal(expectedContent,_tagHelperOutput.Content.GetContent());
        }
        
        [Fact]
        public void GivenShowAction_RenderSummaryListRowWithBlankAction()
        {
            var tagHelper = new GdsKeyValueTagHelper(HtmlEncoder.Default)
            {
                Key = "My Key",
                Value = "My Value",
                ShowAction = true
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);
            
            Assert.Contains("<dd class=\"govuk-summary-list__actions\"></dd>",_tagHelperOutput.Content.GetContent());
        }
        
        [Fact]
        public void GivenShowActionFalse_DoNotRenderBlankAction()
        {
            var tagHelper = new GdsKeyValueTagHelper(HtmlEncoder.Default)
            {
                Key = "My Key",
                Value = "My Value"
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);
            
            Assert.DoesNotContain("<dd class=\"govuk-summary-list__actions\"></dd>",_tagHelperOutput.Content.GetContent());
        }
    }
}