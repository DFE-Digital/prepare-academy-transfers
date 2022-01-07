﻿using System;
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
    public class OtherFactorsTests : PageTests
    {
        private readonly OtherFactors _subject;

        protected OtherFactorsTests()
        {
            _subject = new OtherFactors(ProjectRepository.Object);
        }

        public class GetTests : OtherFactorsTests
        {
            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;

                var response = await _subject.OnGetAsync();

                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewModel);
            }
        }

        public class PostTests : OtherFactorsTests
        {
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
            public async void GivenUrnAndOtherFactors_RedirectsToTheSummaryPage()
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

                ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/Benefits/Index",
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
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var otherFactors = new List<OtherFactorsItemViewModel>();
                _subject.OtherFactorsViewModel.OtherFactorsVm = otherFactors;
                _subject.Urn = ProjectErrorUrn;

                var response = await _subject.OnPostAsync();

                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewModel);
            }

            [Fact]
            public async void GivenUpdateReturnsError_DisplayErrorPage()
            {
                const string errorMessage = "Project Update failed";
                ProjectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                    .ReturnsAsync(new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            StatusCode = System.Net.HttpStatusCode.NotFound,
                            ErrorMessage = errorMessage
                        }
                    });
                var otherFactors = new List<OtherFactorsItemViewModel>();
                _subject.OtherFactorsViewModel.OtherFactorsVm = otherFactors;
                _subject.Urn = ProjectUrn0001;

                var response = await _subject.OnPostAsync();

                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(errorMessage, viewModel);
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
                 ControllerTestHelpers.AssertResultRedirectsToPage(result, Links.HeadteacherBoard.Preview.PageName,
                     new RouteValueDictionary(new {id = ProjectUrn0001}));
             }
        }
    }
}