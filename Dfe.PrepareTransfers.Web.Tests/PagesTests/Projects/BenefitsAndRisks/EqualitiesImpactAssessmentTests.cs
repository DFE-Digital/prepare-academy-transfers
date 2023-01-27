using Data.Models;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks;
using Dfe.PrepareTransfers.Web.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.BenefitsAndRisks
{
    public class EqualitiesImpactAssessmentTests : BaseTests
    {
        private readonly EqualitiesImpactAssessment _subject;

        public EqualitiesImpactAssessmentTests()
        {
            _subject = new EqualitiesImpactAssessment(ProjectRepository.Object);
        }

        public class GetTests : EqualitiesImpactAssessmentTests
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_get_radio_buttons(bool equalitiesImpactAssessmentConsidered)
            {
                var list = new List<RadioButtonViewModel>
                {
                    new RadioButtonViewModel
                    {
                        DisplayName = "Yes",
                        Name = $"{nameof(EqualitiesImpactAssessmentViewModel.EqualitiesImpactAssessmentConsidered)}",
                        Value = "true",
                        Checked = equalitiesImpactAssessmentConsidered is true
                    },
                    new RadioButtonViewModel
                    {
                        DisplayName = "No",
                        Name = $"{nameof(EqualitiesImpactAssessmentViewModel.EqualitiesImpactAssessmentConsidered)}",
                        Value = "false",
                        Checked = equalitiesImpactAssessmentConsidered is false
                    }
                };

                FoundProjectFromRepo.Benefits.EqualitiesImpactAssessmentConsidered = equalitiesImpactAssessmentConsidered;

                var result = await _subject.OnGetAsync();

                Assert.Multiple(
                   () => Assert.IsType<PageResult>(result),
                   () => Assert.Equivalent(list, _subject.RadioButtonsYesNo));
            }
        }

        [Fact]
        public async Task Should_get_trust_name()
        {
            var result = await _subject.OnGetAsync();

            Assert.Multiple(
               () => Assert.IsType<PageResult>(result),
               () => Assert.Equivalent(FoundProjectFromRepo.IncomingTrustName, _subject.IncomingTrustName));
        }


        public class PostTests : EqualitiesImpactAssessmentTests
        {
            [Fact]
            public async Task Should_redirect_to_BenefitsAndRisks_Index()
            {
                _subject.Urn = ProjectUrn0001;
                var result = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(result, "/Projects/BenefitsAndRisks/Index",
                  new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_update_api(bool equalitiesImpactAssessmentConsidered)
            {
                _subject.EqualitiesImpactAssessmentViewModel.EqualitiesImpactAssessmentConsidered = equalitiesImpactAssessmentConsidered;

                var result = await _subject.OnPostAsync();

                ProjectRepository.Verify(pr => pr.Update(It.Is<Project>(p => p.Benefits.EqualitiesImpactAssessmentConsidered == equalitiesImpactAssessmentConsidered)),
                    Times.Once);
            }

            [Fact]
            public async Task Should_fail_validation_and_set_radio_buttons()
            {
                _subject.ModelState.AddModelError("", "modelstate is not valid");

                var list = new List<RadioButtonViewModel>
                {
                    new RadioButtonViewModel
                    {
                        DisplayName = "Yes",
                        Name = $"{nameof(EqualitiesImpactAssessmentViewModel.EqualitiesImpactAssessmentConsidered)}",
                        Value = "true",
                        Checked = false
                    },
                    new RadioButtonViewModel
                    {
                        DisplayName = "No",
                        Name = $"{nameof(EqualitiesImpactAssessmentViewModel.EqualitiesImpactAssessmentConsidered)}",
                        Value = "false",
                        Checked = false
                    }
                };

                var result = await _subject.OnPostAsync();

                Assert.Multiple(
                    () => Assert.IsType<PageResult>(result),
                    () => Assert.Equivalent(list, _subject.RadioButtonsYesNo));
            }
        }
    }
}