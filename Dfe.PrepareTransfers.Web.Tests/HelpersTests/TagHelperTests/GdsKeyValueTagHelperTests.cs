using Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.HelpersTests.TagHelperTests
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
                "<dt class=\"govuk-summary-list__key\">My Key</dt><dd class=\"dfe-summary-list__value--width-50 govuk-summary-list__value\">My Value</dd>";

            Assert.Equal("div", _tagHelperOutput.TagName);
            Assert.Equal(expectedContent, _tagHelperOutput.Content.GetContent());
        }

        [Fact]
        public void GivenKeyValuesWithHtml_RenderSummaryListRowWithHtml()
        {
            var tagHelper = new GdsKeyValueTagHelper(HtmlEncoder.Default)
            {
                Key = "My Key",
                Value = "<a>test</a>"
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            var expectedContent =
                "<dt class=\"govuk-summary-list__key\">My Key</dt><dd class=\"dfe-summary-list__value--width-50 govuk-summary-list__value\"><a>test</a></dd>";

            Assert.Equal("div", _tagHelperOutput.TagName);
            Assert.Equal(expectedContent, _tagHelperOutput.Content.GetContent());
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

            Assert.DoesNotContain("<dd class=\"govuk-summary-list__actions\"></dd>", _tagHelperOutput.Content.GetContent());
        }
    }
}