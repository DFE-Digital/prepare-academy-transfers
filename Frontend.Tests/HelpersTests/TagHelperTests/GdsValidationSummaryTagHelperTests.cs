using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.Helpers.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Frontend.Tests.HelpersTests.TagHelperTests
{
    public class GdsValidationSummaryTagHelperTests
    {
        public class ProcessTests
        {
            [Fact]
            public void GivenNoModelStateErrors_ShouldNotRender()
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new object(),
                };
                var tagHelper = new GdsValidationSummaryTagHelper()
                {
                    ViewContext = new ViewContext {ViewData = viewData}
                };

                var tagHelperContext = new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(),
                    Guid.NewGuid().ToString("N"));
                var tagHelperOutput = new TagHelperOutput("div",
                    new TagHelperAttributeList(),
                    (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        var helperContent = tagHelperContent.SetHtmlContent("Test");
                        return Task.FromResult(helperContent);
                    });

                tagHelper.Process(tagHelperContext, tagHelperOutput);

                Assert.Null(tagHelperOutput.TagName);
                Assert.Equal(string.Empty, tagHelperOutput.Content.GetContent());
                Assert.Empty(tagHelperOutput.Attributes);
            }
            
            [Fact]
            public void GivenModelWithNoModelStateErrors_ShouldNotRender()
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new KeyValuePair<string, object>("testKey", "testValue")
                };
                var tagHelper = new GdsValidationSummaryTagHelper()
                {
                    ViewContext = new ViewContext {ViewData = viewData}
                };

                var tagHelperContext = new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(),
                    Guid.NewGuid().ToString("N"));
                var tagHelperOutput = new TagHelperOutput("div",
                    new TagHelperAttributeList(),
                    (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        var helperContent = tagHelperContent.SetHtmlContent("Test");
                        return Task.FromResult(helperContent);
                    });

                tagHelper.Process(tagHelperContext, tagHelperOutput);

                Assert.Null(tagHelperOutput.TagName);
                Assert.Equal(string.Empty, tagHelperOutput.Content.GetContent());
                Assert.Empty(tagHelperOutput.Attributes);
            }

            [Fact]
            public void GivenModelStateError_ShouldRenderSummary()
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new object(),
                };
                viewData.ModelState.AddModelError("testField", "Test error");
                var tagHelper = new GdsValidationSummaryTagHelper()
                {
                    ViewContext = new ViewContext {ViewData = viewData}
                };

                var tagHelperContext = new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(),
                    Guid.NewGuid().ToString("N"));
                var tagHelperOutput = new TagHelperOutput("div",
                    new TagHelperAttributeList(),
                    (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        var helperContent = tagHelperContent.SetHtmlContent("Test");
                        return Task.FromResult(helperContent);
                    });

                tagHelper.Process(tagHelperContext, tagHelperOutput);

                var expectedOutput = 
                    "<div class='govuk-grid-row'>" +
                        "<div class='govuk-grid-column-full'>" +
                            "<div class='govuk-error-summary' aria-labelledby='error-summary-title' role='alert' " +
                                "data-module='govuk-error-summary' data-ga-event-form='error' data-qa='error'>" +
                                "<h2 class='govuk-error-summary__title' id='error-summary-title' data-qa='error__heading'>There is a problem" +
                                "</h2>" +
                                "<div class='govuk-error-summary__body'>" +
                                    "<ul class='govuk-list govuk-error-summary__list'>" +
                                        "<li><a href='#testField' data-qa='error_text'>Test error</a></li>" +
                                    "</ul>" +
                                "</div>" +
                            "</div>" +
                        "</div>" +
                    "</div>";

                Assert.Equal("div", tagHelperOutput.TagName);
                Assert.Equal(expectedOutput, tagHelperOutput.Content.GetContent());
            }

            [Fact]
            public void GivenMultipleModelStateErrors_ShouldRenderSummary()
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new object(),
                };
                viewData.ModelState.AddModelError("testField", "Test error 1");
                viewData.ModelState.AddModelError("testField", "Test error 2");
                viewData.ModelState.AddModelError("testField2", "Test field 2 error 1");
                viewData.ModelState.AddModelError("testField3", "Test field 3 error 1");
                viewData.ModelState.AddModelError("testField3", "Test field 3 error 2");
                viewData.ModelState.AddModelError("testField3", "Test field 3 error 3");
                var tagHelper = new GdsValidationSummaryTagHelper()
                {
                    ViewContext = new ViewContext {ViewData = viewData}
                };

                var tagHelperContext = new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(),
                    Guid.NewGuid().ToString("N"));
                var tagHelperOutput = new TagHelperOutput("div",
                    new TagHelperAttributeList(),
                    (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        var helperContent = tagHelperContent.SetHtmlContent("Test");
                        return Task.FromResult(helperContent);
                    });

                tagHelper.Process(tagHelperContext, tagHelperOutput);

                var expectedOutput = 
                    "<div class='govuk-grid-row'>" +
                        "<div class='govuk-grid-column-full'>" +
                            "<div class='govuk-error-summary' aria-labelledby='error-summary-title' role='alert' " +
                                "data-module='govuk-error-summary' data-ga-event-form='error' data-qa='error'>" +
                                "<h2 class='govuk-error-summary__title' id='error-summary-title' data-qa='error__heading'>There is a problem" +
                                "</h2>" +
                                "<div class='govuk-error-summary__body'>" +
                                    "<ul class='govuk-list govuk-error-summary__list'>" +
                                        "<li><a href='#testField' data-qa='error_text'>Test error 1</a></li>" +
                                        "<li><a href='#testField' data-qa='error_text'>Test error 2</a></li>" +
                                        "<li><a href='#testField2' data-qa='error_text'>Test field 2 error 1</a></li>" +
                                        "<li><a href='#testField3' data-qa='error_text'>Test field 3 error 1</a></li>" +
                                        "<li><a href='#testField3' data-qa='error_text'>Test field 3 error 2</a></li>" +
                                        "<li><a href='#testField3' data-qa='error_text'>Test field 3 error 3</a></li>" +
                                    "</ul>" +
                                "</div>" +
                            "</div>" +
                        "</div>" +
                    "</div>";

                Assert.Equal("div", tagHelperOutput.TagName);
                Assert.Equal(expectedOutput, tagHelperOutput.Content.GetContent());
            }
        }
    }
}