using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Frontend.Pages.TaskList.KeyStage5Performance;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.TaskList
{
    public class KeyStage5PerformanceTests
    {
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly Mock<IProjects> _projectRepository;
        private readonly KeyStage5Performance _subject;
        private readonly EducationPerformance _foundEducationPerformance;
        private readonly Project _foundProject;
        private readonly Academy _foundOutgoingAcademy;
        private readonly GetInformationForProjectResponse _foundInformationForProject;

        public KeyStage5PerformanceTests()
        {
            _projectRepository = new Mock<IProjects>();
            _getInformationForProject = new Mock<IGetInformationForProject>();

            _foundEducationPerformance = new EducationPerformance
            {
                KeyStage5Performance = new List<KeyStage5> {new KeyStage5 {Year = "2019"}}
            };

            _foundProject = new Project {Urn = "1234"};

            _foundOutgoingAcademy = new Academy
            {
                Name = "Academy name", Urn = "Urn", LocalAuthorityName = "LA Name"
            };

            _foundInformationForProject = new GetInformationForProjectResponse
            {
                Project = _foundProject,
                EducationPerformance = _foundEducationPerformance,
                OutgoingAcademy = _foundOutgoingAcademy
            };

            _getInformationForProject
                .Setup(s => s.Execute(It.IsAny<string>()))
                .ReturnsAsync(
                    _foundInformationForProject
                );

            _projectRepository.Setup(s => s.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = _foundProject
                });

            _subject = new KeyStage5Performance(_getInformationForProject.Object, _projectRepository.Object);
        }

        public class OnGetAsyncTests : KeyStage5PerformanceTests
        {
            [Theory]
            [InlineData("1234")]
            [InlineData("4321")]
            public async void OnGet_GetInformationForProjectId(string id)
            {
                await _subject.OnGetAsync(id);

                _getInformationForProject.Verify(s => s.Execute(id));
            }

            [Fact]
            public async void OnGet_AssignsCorrectValuesToViewModel()
            {
                var result = await _subject.OnGetAsync("1234");

                Assert.IsType<PageResult>(result);
                Assert.Equal(_foundProject.Urn, _subject.ProjectUrn);
                Assert.Equal(_foundEducationPerformance, _subject.EducationPerformance);
                Assert.Equal(_foundOutgoingAcademy.Urn, _subject.OutgoingAcademyUrn);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                const string additionalInformation = "some additional info";
                _foundInformationForProject.Project.KeyStage5PerformanceAdditionalInformation = additionalInformation;

                await _subject.OnGetAsync("1234");

                Assert.Equal(additionalInformation, _subject.AdditionalInformation.AdditionalInformation);
                Assert.Equal(_foundProject.Urn, _subject.AdditionalInformation.Urn);
            }

            [Fact]
            public async void GivenReturnToPreview_UpdatesTheViewModel()
            {
                await _subject.OnGetAsync("123", false, true);

                Assert.True(_subject.ReturnToPreview);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage5Performance>(
                        _getInformationForProject.Object, _projectRepository.Object);

                _getInformationForProject.Setup(s => s.Execute(_foundProject.Urn)).ReturnsAsync(
                    new GetInformationForProjectResponse
                    {
                        ResponseError = new ServiceResponseError
                            {ErrorCode = ErrorCode.NotFound, ErrorMessage = "Error message"}
                    }
                );

                var response = await pageModel.OnGetAsync(_foundProject.Urn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error message", viewResult.Model);
            }
        }

        public class OnPostAsyncTests : KeyStage5PerformanceTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnPostAsync("1234", string.Empty, false);

                _projectRepository.Verify(r => r.GetByUrn("1234"), Times.Once);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage5Performance>(
                        _getInformationForProject.Object, _projectRepository.Object);

                _projectRepository.Setup(s => s.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "Error"
                        }
                    });


                var response = await pageModel.OnPostAsync("1234", string.Empty, false);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInformation = "some additional info";

                var response = await _subject.OnPostAsync("1234", additionalInformation, false);

                var redirectToPageResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal("KeyStage5Performance", redirectToPageResponse.PageName);
                Assert.Equal("OnGetAsync", redirectToPageResponse.PageHandler);
                Assert.Equal(additionalInformation, _foundProject.KeyStage5PerformanceAdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                const string additionalInfo = "test info";

                await _subject.OnPostAsync("1234", additionalInfo, false);
                _projectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.KeyStage5PerformanceAdditionalInformation == additionalInfo
                )));
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToThePreviewPage()
            {
                var response = await _subject.OnPostAsync("1234", "", true);

                var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal(Links.HeadteacherBoard.Preview.PageName, redirectResponse.PageName);
                Assert.Equal("1234", redirectResponse.RouteValues["id"]);
            }
        }
    }
}