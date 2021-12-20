using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Frontend.Pages.TaskList.KeyStage4Performance;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.TaskList
{
    public class KeyStage4PerformanceTests : PageTests
    {
        private readonly KeyStage4Performance _subject;

        public KeyStage4PerformanceTests()
        {
            //arrange
            FoundInformationForProject.EducationPerformance = new EducationPerformance
            {
                KeyStage4Performance = new List<KeyStage4>
                {
                    new KeyStage4
                    {
                        Year = "2019-2020",
                        SipNumberofpupilsprogress8 = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = "20.5",
                            Disadvantaged = "10.5"
                        }
                    },
                    new KeyStage4
                    {
                        Year = "2018-2019",
                        SipNumberofpupilsprogress8 = new DisadvantagedPupilsResult
                        {
                            NotDisadvantaged = "40.8",
                            Disadvantaged = "30.4"
                        }
                    }
                }
            };

            _subject = new KeyStage4Performance(GetInformationForProject.Object, ProjectRepository.Object);
        }

        public class OnGetAsyncTests : KeyStage4PerformanceTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync(ProjectUrn0001);

                GetInformationForProject.Verify(r => r.Execute(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenExistingProject_AssignsItToThePageModel()
            {
                var response = await _subject.OnGetAsync(ProjectUrn0001);

                Assert.IsType<PageResult>(response);
                Assert.Equal(ProjectUrn0001, _subject.ProjectUrn);
                Assert.Equal(AcademyUrn, _subject.OutgoingAcademyUrn);
                Assert.Equal(2, _subject.EducationPerformance.KeyStage4Performance.Count);
                Assert.Equal("2019-2020", _subject.EducationPerformance.KeyStage4Performance[0].Year);
                Assert.Equal("2018-2019", _subject.EducationPerformance.KeyStage4Performance[1].Year);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                const string additionalInformation = "some additional info";
                FoundInformationForProject.Project.KeyStage4PerformanceAdditionalInformation = additionalInformation;
                GetInformationForProject.Setup(s => s.Execute(ProjectUrn0001))
                    .ReturnsAsync(FoundInformationForProject);

                await _subject.OnGetAsync(ProjectUrn0001);

                Assert.Equal(additionalInformation, _subject.AdditionalInformation.AdditionalInformation);
                Assert.Equal(ProjectUrn0001, _subject.AdditionalInformation.Urn);
            }

            [Fact]
            public async void GivenReturnToPreview_UpdatesTheViewModel()
            {
                await _subject.OnGetAsync(ProjectUrn0001, false, true);

                Assert.True(_subject.ReturnToPreview);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage4Performance>(
                        GetInformationForProject.Object, ProjectRepository.Object);

                var response = await pageModel.OnGetAsync(ProjectErrorUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }

        public class OnPostAsyncTests : KeyStage4PerformanceTests
        {
            private readonly Project _foundProject;

            public OnPostAsyncTests()
            {
                _foundProject = new Project
                {
                    Urn = ProjectUrn0001
                };

                ProjectRepository.Setup(s => s.GetByUrn(ProjectUrn0001)).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Result = _foundProject
                    });

                ProjectRepository.Setup(s => s.GetByUrn(ProjectErrorUrn)).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "Error"
                        }
                    });
            }

            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnPostAsync(ProjectUrn0001, string.Empty, false);

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage4Performance>(
                        GetInformationForProject.Object, ProjectRepository.Object);

                var response = await pageModel.OnPostAsync(ProjectErrorUrn, string.Empty, false);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInformation = "some additional info";

                var response = await _subject.OnPostAsync(ProjectUrn0001, additionalInformation, false);

                var redirectToPageResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal("KeyStage4Performance", redirectToPageResponse.PageName);
                Assert.Equal("OnGetAsync", redirectToPageResponse.PageHandler);
                Assert.Equal(additionalInformation, _foundProject.KeyStage4PerformanceAdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                const string additionalInfo = "test info";

                await _subject.OnPostAsync(ProjectUrn0001, additionalInfo, false);
                ProjectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.KeyStage4PerformanceAdditionalInformation == additionalInfo
                )));
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToThePreviewPage()
            {
                var response = await _subject.OnPostAsync(ProjectUrn0001, "", true);

                var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal(Links.HeadteacherBoard.Preview.PageName, redirectResponse.PageName);
                Assert.Equal(ProjectUrn0001, redirectResponse.RouteValues["id"]);
            }
        }
    }
}