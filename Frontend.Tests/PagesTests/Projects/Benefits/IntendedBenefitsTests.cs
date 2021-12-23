using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Pages.Projects.Benefits;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.Benefits
{
    public class IntendedBenefitsTests : PageTests
    {
        private readonly IntendedBenefits _subject;

        public IntendedBenefitsTests()
        {
            _subject = new IntendedBenefits(ProjectRepository.Object);
        }

        public class GetTests : IntendedBenefitsTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                _subject.Urn = ProjectUrn0001;
                await _subject.OnGetAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenReturnToPreview_KeepsValue()
            {
                _subject.ReturnToPreview = true;

                await _subject.OnGetAsync();

                Assert.True(_subject.ReturnToPreview);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;
                var response = await _subject.OnGetAsync();

                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewModel);
            }
        }

        public class PostTests : IntendedBenefitsTests
        {
            [Fact]
            public async void GivenUrnAndIntendedBenefits_UpdatesTheProject()
            {
                var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                {
                    TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                    TransferBenefits.IntendedBenefit.StrengtheningGovernance
                };
                _subject.IntendedBenefitsViewModel.SelectedIntendedBenefits = intendedBenefits;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(
                        project => project.Benefits.IntendedBenefits.All(intendedBenefits.Contains))));
            }

            [Fact]
            public async void GivenOtherBenefitAndItsDetails_UpdatesTheProject()
            {
                var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                {
                    TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                    TransferBenefits.IntendedBenefit.StrengtheningGovernance,
                    TransferBenefits.IntendedBenefit.Other
                };
                _subject.IntendedBenefitsViewModel.SelectedIntendedBenefits = intendedBenefits;
                _subject.IntendedBenefitsViewModel.OtherBenefit = "Other benefit";

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(
                        project => project.Benefits.IntendedBenefits.All(intendedBenefits.Contains) &&
                                   project.Benefits.OtherIntendedBenefit == "Other benefit")
                    )
                );
            }

            [Fact]
            public async void GivenUrnAndIntendedBenefits_RedirectsToTheSummaryPage()
            {
                _subject.Urn = ProjectUrn0001;
                var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                {
                    TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                    TransferBenefits.IntendedBenefit.StrengtheningGovernance
                };
                _subject.IntendedBenefitsViewModel.SelectedIntendedBenefits = intendedBenefits;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/Benefits/Index",
                    new RouteValueDictionary(new {Urn = ProjectUrn0001}));
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;
                var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                {
                    TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                    TransferBenefits.IntendedBenefit.Other
                };
                _subject.IntendedBenefitsViewModel.SelectedIntendedBenefits = intendedBenefits;

                var response = await _subject.OnPostAsync();

                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewModel);
            }

            [Fact]
            public async void GivenUpdateReturnsError_DisplayErrorPage()
            {
                var errorMessage = "Project update failed";
                ProjectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                    .ReturnsAsync(new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            StatusCode = System.Net.HttpStatusCode.NotFound,
                            ErrorMessage = errorMessage
                        }
                    });
                var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                {
                    TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                    TransferBenefits.IntendedBenefit.CentralFinanceTeamAndSupport
                };
                _subject.IntendedBenefitsViewModel.SelectedIntendedBenefits = intendedBenefits;

                var response = await _subject.OnPostAsync();

                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(errorMessage, viewModel);
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectToThePreviewPage()
            {
                _subject.IntendedBenefitsViewModel.SelectedIntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                {
                    TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                    TransferBenefits.IntendedBenefit.StrengtheningGovernance
                };
                _subject.Urn = ProjectUrn0001;
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {id = ProjectUrn0001}));
            }
        }
    }
}