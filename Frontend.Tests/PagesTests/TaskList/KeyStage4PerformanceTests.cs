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
    public class KeyStage4PerformanceTests
    {
        private const string ProjectErrorUrn = "errorUrn";
        private const string ProjectUrn = "0001";
        private const string AcademyUrn = "1234";
        private const string AcademyName = "Academy Name";
        private const string LaName = "LA Name";
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly Mock<IProjects> _projectRepository;
        private readonly GetInformationForProjectResponse _foundInformationForProject;
        private readonly KeyStage4Performance _subject;

        public KeyStage4PerformanceTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();
            _foundInformationForProject = new GetInformationForProjectResponse
            {
                Project = new Project
                {
                    Urn = ProjectUrn
                },
                OutgoingAcademy = new Academy
                {
                    Urn = AcademyUrn,
                    LocalAuthorityName = LaName,
                    Name = AcademyName
                },
                EducationPerformance = new EducationPerformance
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
                }
            };
            
            _projectRepository = new Mock<IProjects>();

            _getInformationForProject.Setup(s => s.Execute(ProjectUrn))
                .ReturnsAsync(
                    _foundInformationForProject
                );

            _getInformationForProject.Setup(s => s.Execute(ProjectErrorUrn))
                .ReturnsAsync(
                    new GetInformationForProjectResponse
                    {
                        ResponseError = new ServiceResponseError
                        {
                            ErrorMessage = "Error"
                        }
                    });

            _subject = new KeyStage4Performance(_getInformationForProject.Object, _projectRepository.Object);
        }

        public class OnGetAsyncTests : KeyStage4PerformanceTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync(ProjectUrn);

                _getInformationForProject.Verify(r => r.Execute(ProjectUrn), Times.Once);
            }
            
            [Fact]
            public async void GivenExistingProject_AssignsItToThePageModel()
            {
                var response = await _subject.OnGetAsync(ProjectUrn);

                Assert.IsType<PageResult>(response);
                Assert.Equal(ProjectUrn, _subject.ProjectUrn);
                Assert.Equal(AcademyUrn, _subject.OutgoingAcademyUrn);
                Assert.Equal(AcademyName, _subject.OutgoingAcademyName);
                Assert.Equal(LaName, _subject.LocalAuthorityName);
                Assert.Equal(3, _subject.KeyStage4Results.Count);
                Assert.Equal("2019-2020", _subject.KeyStage4Results[0].Year);
                Assert.Equal("2018-2019", _subject.KeyStage4Results[1].Year);
                Assert.Equal("2017-2018", _subject.KeyStage4Results[2].Year);
            }
            
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                const string additionalInformation = "some additional info";
                _foundInformationForProject.Project.KeyStage4PerformanceAdditionalInformation = additionalInformation;
                _getInformationForProject.Setup(s => s.Execute(ProjectUrn))
                    .ReturnsAsync(_foundInformationForProject);

                await _subject.OnGetAsync(ProjectUrn);

                Assert.Equal(additionalInformation, _subject.AdditionalInformation.AdditionalInformation);
                Assert.Equal(ProjectUrn, _subject.AdditionalInformation.Urn);
            }
            
            [Fact]
            public async void GivenReturnToPreview_UpdatesTheViewModel()
            {
                await _subject.OnGetAsync(ProjectUrn, false, true);

                Assert.True(_subject.ReturnToPreview);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage4Performance>(
                        _getInformationForProject.Object, _projectRepository.Object);

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
                    Urn = ProjectUrn
                };

                _projectRepository.Setup(s => s.GetByUrn(ProjectUrn)).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Result = _foundProject
                    });

                _projectRepository.Setup(s => s.GetByUrn(ProjectErrorUrn)).ReturnsAsync(
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
                await _subject.OnPostAsync(ProjectUrn, string.Empty, false);

                _projectRepository.Verify(r => r.GetByUrn(ProjectUrn), Times.Once);
            }
            
            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage4Performance>(
                        _getInformationForProject.Object, _projectRepository.Object);

                var response = await pageModel.OnPostAsync(ProjectErrorUrn, string.Empty, false);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
            
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInformation = "some additional info";

                var response = await _subject.OnPostAsync(ProjectUrn, additionalInformation, false);

                var redirectToPageResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal("KeyStage4Performance", redirectToPageResponse.PageName);
                Assert.Equal("OnGetAsync", redirectToPageResponse.PageHandler);
                Assert.Equal(additionalInformation, _foundProject.KeyStage4PerformanceAdditionalInformation);
            }
            
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                const string additionalInfo = "test info";

                await _subject.OnPostAsync(ProjectUrn, additionalInfo, false);
                _projectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.KeyStage4PerformanceAdditionalInformation == additionalInfo
                )));
            }
                
            [Fact]
            public async void GivenReturnToPreview_RedirectsToThePreviewPage()
            {
                var response = await _subject.OnPostAsync(ProjectUrn, "", true);

                var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal(Links.HeadteacherBoard.Preview.PageName, redirectResponse.PageName);
                Assert.Equal(ProjectUrn, redirectResponse.RouteValues["id"]);
            }
        }
    }
}