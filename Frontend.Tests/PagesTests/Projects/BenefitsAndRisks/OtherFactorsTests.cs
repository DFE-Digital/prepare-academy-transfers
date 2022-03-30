using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Pages.Projects.BenefitsAndRisks;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.BenefitsAndRisks
{
    public class OtherFactorsTests : BaseTests
    {
        private readonly OtherFactors _subject;

        public OtherFactorsTests()
        {
            _subject = new OtherFactors(ProjectRepository.Object);
        }


        [Fact]
        public async void GivenUrnAndNoOtherFactors_UpdateTheProject()
        {
            _subject.Urn = ProjectUrn0001;

            await _subject.OnPostAsync();

            ProjectRepository.Verify(r =>
                r.Update(It.Is<Project>(project => !project.Benefits.OtherFactors.Any()))
            );
        }

        [Fact]
        public async void GivenUrnAndAllOtherFactors_UpdateTheProject()
        {
            Func<Project, bool> assertOtherFactorsEqual = project =>
            {
                var projectOtherFactors = project.Benefits.OtherFactors;
                var highProfile = projectOtherFactors[TransferBenefits.OtherFactor.HighProfile];
                var complexIssues =
                    projectOtherFactors[TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues];
                var finance = projectOtherFactors[TransferBenefits.OtherFactor.FinanceAndDebtConcerns];

                return highProfile == "High profile" &&
                       complexIssues == "Complex issues" &&
                       finance == "Finance concerns";
            };

            var otherFactors = new List<OtherFactorsItemViewModel>
            {
                new OtherFactorsItemViewModel
                {
                    OtherFactor = TransferBenefits.OtherFactor.HighProfile,
                    Checked = true,
                    Description = "High profile"
                },
                new OtherFactorsItemViewModel
                {
                    OtherFactor = TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                    Checked = true,
                    Description = "Finance concerns"
                },
                new OtherFactorsItemViewModel
                {
                    OtherFactor = TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                    Checked = true,
                    Description = "Complex issues"
                },
            };
            _subject.OtherFactorsViewModel.OtherFactorsVm = otherFactors;
            _subject.Urn = ProjectUrn0001;

            await _subject.OnPostAsync();

            ProjectRepository.Verify(r =>
                r.Update(It.Is<Project>(project => assertOtherFactorsEqual(project)))
            );
        }

        [Fact]
        public async void GivenUrnAndAllOtherFactors_RedirectsToTheFirstPage()
        {
            var otherFactors =
                Enum.GetValues(typeof(TransferBenefits.OtherFactor))
                    .Cast<TransferBenefits.OtherFactor>()
                    .Where(otherFactor => otherFactor != TransferBenefits.OtherFactor.Empty)
                    .Select(otherFactor => new OtherFactorsItemViewModel()
                    {
                        OtherFactor = otherFactor,
                        Checked = true,
                        Description = "test description"
                    }).ToList();
            _subject.OtherFactorsViewModel.OtherFactorsVm = otherFactors;
            _subject.Urn = ProjectUrn0001;

            var response = await _subject.OnPostAsync();

            ControllerTestHelpers.AssertResultRedirectsToPage(response,
                "/Projects/BenefitsAndRisks/HighProfileTransfer",
                new RouteValueDictionary(new {Urn = ProjectUrn0001}));
        }

        [Theory]
        [InlineData("HighProfile")]
        [InlineData("ComplexLandAndBuildingIssues")]
        [InlineData("FinanceAndDebtConcerns")]
        public async void GivenUrnAndOtherFactor_UpdatesTheProjectCorrectly(string otherFactorString)
        {
            var otherFactors = new List<OtherFactorsItemViewModel>
            {
                new OtherFactorsItemViewModel
                {
                    OtherFactor =
                        (TransferBenefits.OtherFactor) Enum.Parse(typeof(TransferBenefits.OtherFactor),
                            otherFactorString),
                    Checked = true,
                    Description = "test Description"
                }
            };
            _subject.OtherFactorsViewModel.OtherFactorsVm = otherFactors;
            _subject.Urn = ProjectUrn0001;

            await _subject.OnPostAsync();
            ProjectRepository.Verify(r => r.Update(It.Is<Project>(
                project => project.Benefits.OtherFactors.Keys.Count == 1 &&
                           project.Benefits.OtherFactors[otherFactors[0].OtherFactor] ==
                           "test Description"
            )));
        }

        [Fact]
        public async void GivenReturnToPreview_RedirectToThePreviewPage()
        {
            var otherFactors = new List<OtherFactorsItemViewModel>
            {
                new OtherFactorsItemViewModel
                {
                    OtherFactor =
                        (TransferBenefits.OtherFactor) Enum.Parse(typeof(TransferBenefits.OtherFactor),
                            "HighProfile"),
                    Checked = true,
                    Description = "Meow"
                }
            };
            _subject.OtherFactorsViewModel.OtherFactorsVm = otherFactors;
            _subject.Urn = ProjectUrn0001;
            _subject.ReturnToPreview = true;

            var result = await _subject.OnPostAsync();
            ControllerTestHelpers.AssertResultRedirectsToPage(result, "/Projects/BenefitsAndRisks/HighProfileTransfer",
                new RouteValueDictionary(new {Urn = ProjectUrn0001}));
        }

        [Fact]
        public void GivenNoAvailablePages_GetPage_GoToIndex()
        {
            var otherFactors = new Dictionary<TransferBenefits.OtherFactor, string>();
            var page = OtherFactors.GetPage(new List<TransferBenefits.OtherFactor>(0), otherFactors);
            Assert.Equal("/Projects/BenefitsAndRisks/Index", page);
        }

        [Fact]
        public void GivenNoAvailablePagesWithBack_GetPage_GoToOtherFactors()
        {
            var otherFactors = new Dictionary<TransferBenefits.OtherFactor, string>();
            var page = OtherFactors.GetPage(new List<TransferBenefits.OtherFactor>(0), otherFactors, true);
            Assert.Equal("/Projects/BenefitsAndRisks/OtherFactors", page);
        }

        [Theory]
        [InlineData(TransferBenefits.OtherFactor.HighProfile)]
        [InlineData(TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues)]
        [InlineData(TransferBenefits.OtherFactor.FinanceAndDebtConcerns)]
        [InlineData(TransferBenefits.OtherFactor.OtherRisks)]
        public void GivenAvailablePage_GetPage_GoToCorrectPage(TransferBenefits.OtherFactor otherFactor)
        {
            var otherFactors = new Dictionary<TransferBenefits.OtherFactor, string> {{otherFactor, string.Empty}};
            var page = OtherFactors.GetPage(new List<TransferBenefits.OtherFactor>(0), otherFactors, true);
            switch (page)
            {
                case "HighProfile":
                    Assert.Equal("/Projects/BenefitsAndRisks/HighProfileTransfer", page);
                    break;
                case "ComplexLandAndBuildingIssues":
                    Assert.Equal("/Projects/BenefitsAndRisks/ComplexLandAndBuilding", page);
                    break;
                case "FinanceAndDebtConcerns":
                    Assert.Equal("/Projects/BenefitsAndRisks/FinanceAndDebt", page);
                    break;
                case "OtherRisks":
                    Assert.Equal("/Projects/BenefitsAndRisks/OtherRisks", page);
                    break;
            }
        }

        [Fact]
        public void GivenMultipleAvailablePages_GetPage_GoToCorrectPage()
        {
            var otherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
            {
                {TransferBenefits.OtherFactor.FinanceAndDebtConcerns, string.Empty},
                {TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues, string.Empty}
            };
            var availablePages = new List<TransferBenefits.OtherFactor>()
            {
                TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues
            };
            var page = OtherFactors.GetPage(availablePages, otherFactors);
            Assert.Equal("/Projects/BenefitsAndRisks/FinanceAndDebt", page);
        }
    }
}