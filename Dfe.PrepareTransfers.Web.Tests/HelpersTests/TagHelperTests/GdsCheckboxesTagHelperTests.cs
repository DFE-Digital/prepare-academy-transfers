using Dfe.PrepareTransfers.Web.Helpers.TagHelpers;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.HelpersTests.TagHelperTests
{
    public class GdsCheckboxesTagHelperTests
    {
        private readonly TagHelperContext _tagHelperContext;
        private readonly TagHelperOutput _tagHelperOutput;

        public GdsCheckboxesTagHelperTests()
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
        public void GivenWithoutContainerTrue_DoNotRenderContainer()
        {
            var tagHelper = new GdsCheckBoxesTagHelper()
            {
                Checkboxes = new List<CheckboxViewModel>()
                {
                    new CheckboxViewModel {DisplayName = "Box 1", Name = "Checkbox1" },
                    new CheckboxViewModel {DisplayName = "Box 2", Name = "Checkbox2" }
                },
                WithoutContainer = true
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            Assert.Null(_tagHelperOutput.TagName);
        }

        [Fact]
        public void GivenWithoutContainerFalse_RenderContainer()
        {
            var tagHelper = new GdsCheckBoxesTagHelper()
            {
                Checkboxes = new List<CheckboxViewModel>()
                {
                    new CheckboxViewModel {DisplayName = "Box 1", Name = "Checkbox1" },
                    new CheckboxViewModel {DisplayName = "Box 2", Name = "Checkbox2" }
                },
                WithoutContainer = false
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            Assert.Equal("div", _tagHelperOutput.TagName);
            Assert.Equal("govuk-checkboxes", _tagHelperOutput.Attributes["class"].Value);
            Assert.Equal("govuk-checkboxes", _tagHelperOutput.Attributes["data-module"].Value);
        }

        [Fact]
        public void GivenCheckboxes_RenderCheckboxList()
        {
            var tagHelper = new GdsCheckBoxesTagHelper()
            {
                Checkboxes = new List<CheckboxViewModel>()
                {
                    new CheckboxViewModel {DisplayName = "Box 1", Value = "1", Name = "Checkbox1"},
                    new CheckboxViewModel {DisplayName = "Box 2", Value = "2", Checked = true, Name = "Checkbox2"}
                },
                WithoutContainer = false
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            var expectedContent =
                "<div class=\"govuk-checkboxes__item\"><input class=\"govuk-checkboxes__input\" id=\"Checkbox1\" name=\"Checkbox1\" type=\"checkbox\" value=\"1\"><label class=\"govuk-label govuk-checkboxes__label\" for=\"Checkbox1\">Box 1</label></div><div class=\"govuk-checkboxes__item\"><input checked=\"\" class=\"govuk-checkboxes__input\" id=\"2\" name=\"Checkbox2\" type=\"checkbox\" value=\"2\"><label class=\"govuk-label govuk-checkboxes__label\" for=\"2\">Box 2</label></div>";
            Assert.Equal(expectedContent, _tagHelperOutput.Content.GetContent());
        }
        
        [Fact]
        public void GivenCheckboxesWithNestedName_RenderFirstCheckboxIdWithUnderscore()
        {
            var tagHelper = new GdsCheckBoxesTagHelper()
            {
                Checkboxes = new List<CheckboxViewModel>()
                {
                    new CheckboxViewModel {DisplayName = "Box 1", Value = "1", Name = "Checkbox.NestedName"},
                    new CheckboxViewModel {DisplayName = "Box 2", Value = "2", Checked = true, Name = "Checkbox2.NestedName"}
                },
                WithoutContainer = false
            };

            tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            var expectedContent =
                "<div class=\"govuk-checkboxes__item\"><input class=\"govuk-checkboxes__input\" id=\"Checkbox_NestedName\" name=\"Checkbox.NestedName\" type=\"checkbox\" value=\"1\"><label class=\"govuk-label govuk-checkboxes__label\" for=\"Checkbox_NestedName\">Box 1</label></div><div class=\"govuk-checkboxes__item\"><input checked=\"\" class=\"govuk-checkboxes__input\" id=\"2\" name=\"Checkbox2.NestedName\" type=\"checkbox\" value=\"2\"><label class=\"govuk-label govuk-checkboxes__label\" for=\"2\">Box 2</label></div>";
            Assert.Equal(expectedContent, _tagHelperOutput.Content.GetContent());
        }
    }
}