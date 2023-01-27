using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Helpers.TagHelpers;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.HelpersTests.TagHelperTests
{
    public class DateSummaryTagHelperTests
    {
        [Fact]
        public void GivenEmptyDate_RendersCorrectly()
        {
            string dateString = null;
            var dateSummaryTagHelper = new DateSummaryTagHelper()
            {
                Value = dateString
            };
            
            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var tagHelperOutput = new TagHelperOutput("datesummary",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    var helperContent = tagHelperContent.SetHtmlContent("Test");
                    return Task.FromResult(helperContent);
                });

            dateSummaryTagHelper.Process(tagHelperContext, tagHelperOutput);

            Assert.Equal("span", tagHelperOutput.TagName);
            Assert.Equal("Empty", tagHelperOutput.Content.GetContent());
            Assert.Equal("dfe-empty-tag", tagHelperOutput.Attributes["class"].Value);
        }
        
        [Fact]
        public void GivenDate_RendersDateCorrectly()
        {
            var dateString = "01/01/2020";
            var dateSummaryTagHelper = new DateSummaryTagHelper()
            {
                Value = dateString
            };
            
            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var tagHelperOutput = new TagHelperOutput("datesummary",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    var helperContent = tagHelperContent.SetHtmlContent("Test");
                    return Task.FromResult(helperContent);
                });

            dateSummaryTagHelper.Process(tagHelperContext, tagHelperOutput);

            Assert.Equal("span", tagHelperOutput.TagName);
            Assert.Equal("1 January 2020", tagHelperOutput.Content.GetContent());
        }
    }
}