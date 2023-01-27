using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.HelpersTests.TagHelperTests
{
    public class DisplayNoDataTagHelperTests
    {
        private readonly TagHelperContext _tagHelperContext;
        private readonly TagHelperOutput _tagHelperOutput;
        private readonly TagHelperOutput _tagHelperNoContentOutput;
        private readonly DisplayNoDataForEmptyStringTagHelper _tagHelper;
        private const string Content = "<span>Test</span>";
        private const string NoData = "No data";

        public DisplayNoDataTagHelperTests()
        {
            _tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            _tagHelperOutput = new TagHelperOutput("displaynodataforemptystring",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    var helperContent = tagHelperContent.SetHtmlContent(Content);
                    return Task.FromResult(helperContent);
                });
            _tagHelperNoContentOutput = new TagHelperOutput("displaynodataforemptystring",
                new TagHelperAttributeList(),
                (result, encoder) => Task.FromResult(new DefaultTagHelperContent().SetContent(String.Empty)));
            _tagHelper = new DisplayNoDataForEmptyStringTagHelper();
        }

        [Fact]
        public async Task GivenWithoutContent_RenderNoData()
        {
            await _tagHelper.ProcessAsync(_tagHelperContext, _tagHelperNoContentOutput);
            Assert.Equal(NoData, _tagHelperNoContentOutput.Content.GetContent());
        }

        [Fact]
        public async Task GivenWithContent_RenderContent()
        {
            await _tagHelper.ProcessAsync(_tagHelperContext, _tagHelperOutput);
            var content = await _tagHelperOutput.GetChildContentAsync();
            Assert.Equal(Content, content.GetContent());
        }
    }
}