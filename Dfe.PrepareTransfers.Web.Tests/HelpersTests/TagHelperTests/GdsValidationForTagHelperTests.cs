using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Helpers.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.HelpersTests.TagHelperTests
{
    public class GdsValidationForTagHelperTests
    {
        private class TestModel
        {
            public string TestField { get; set; }
        }
        
        public class ProcessTests
        {
            [Fact]
            public void GivenNoModelStateErrors_ShouldNotRender()
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new object()
                };
                var tagHelper = new GdsValidationForTagHelper()
                {
                    ViewContext = new ViewContext {ViewData = viewData}
                };

                var tagHelperContext = new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(),
                    Guid.NewGuid().ToString("N"));
                var tagHelperOutput = new TagHelperOutput("span",
                    new TagHelperAttributeList(),
                    (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        var helperContent = tagHelperContent.SetHtmlContent(string.Empty);
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
                var tagHelper = new GdsValidationForTagHelper()
                {
                    ViewContext = new ViewContext {ViewData = viewData}
                };

                var tagHelperContext = new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(),
                    Guid.NewGuid().ToString("N"));
                var tagHelperOutput = new TagHelperOutput("span",
                    new TagHelperAttributeList(),
                    (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        var helperContent = tagHelperContent.SetHtmlContent(string.Empty);
                        return Task.FromResult(helperContent);
                    });

                tagHelper.Process(tagHelperContext, tagHelperOutput);

                Assert.Null(tagHelperOutput.TagName);
                Assert.Equal(string.Empty, tagHelperOutput.Content.GetContent());
                Assert.Empty(tagHelperOutput.Attributes);
            }
            
            [Fact]
            public void GivenModelWithError_ShouldRender()
            {
                var testModel = new TestModel()
                {
                    TestField = "testKey"
                };
            
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = testModel
                };
                viewData.ModelState.AddModelError(nameof(testModel.TestField), "Test error");
                
                var provider = new EmptyModelMetadataProvider();
                var containerModelExplorer = provider.GetModelExplorerForType(
                    typeof(TestModel),
                    testModel);
                var propertyModelExplorer = containerModelExplorer.GetExplorerForProperty(nameof(TestModel.TestField));
                
                var tagHelper = new GdsValidationForTagHelper()
                {
                    ViewContext = new ViewContext {ViewData = viewData},
                    For = new ModelExpression(nameof(TestModel.TestField), propertyModelExplorer)
                };

                var tagHelperContext = new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(),
                    Guid.NewGuid().ToString("N"));
                var tagHelperOutput = new TagHelperOutput("span",
                    new TagHelperAttributeList(),
                    (result, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        var helperContent = tagHelperContent.SetHtmlContent(string.Empty);
                        return Task.FromResult(helperContent);
                    });

                tagHelper.Process(tagHelperContext, tagHelperOutput);
                
                Assert.Equal("Test error", tagHelperOutput.Content.GetContent());
                Assert.Equal("<span class='govuk-visually-hidden'>Error:</span>", tagHelperOutput.PreContent.GetContent());
                Assert.Equal("span", tagHelperOutput.TagName);
            }
        }
    }
}