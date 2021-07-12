using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Pages;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests
{
    public class KeyStage2PerformanceTests
    {
        private const string ProjectErrorUrn = "errorUrn";
        private const string ProjectUrn = "0001";
        private const string AcademyUrn = "1234";
        private const string AcademyName = "Academy Name";
        private const string LAName = "LA Name";
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly Mock<IProjects> _projectRepository;
        private readonly GetInformationForProjectResponse _foundInformationForProject;
        private readonly KeyStage2Performance _subject;

        public KeyStage2PerformanceTests()
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
                    LocalAuthorityName = LAName,
                    Name = AcademyName
                },
                EducationPerformance = new EducationPerformance
                {
                    KeyStage2Performance = new List<KeyStage2>
                    {
                        new KeyStage2
                        {
                            Year = "test year"
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

            _subject = new KeyStage2Performance(_getInformationForProject.Object, _projectRepository.Object);
        }

        public class OnGetAsyncTests : KeyStage2PerformanceTests
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
                Assert.Equal(LAName, _subject.LocalAuthorityName);
                Assert.Equal("test year", _subject.EducationPerformance.KeyStage2Performance[0].Year);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                const string additionalInformation = "some additional info";
                _foundInformationForProject.Project.KeyStage2PerformanceAdditionalInformation = additionalInformation;
                _getInformationForProject.Setup(s => s.Execute(ProjectUrn))
                    .ReturnsAsync(_foundInformationForProject);

                await _subject.OnGetAsync(ProjectUrn);

                Assert.Equal(additionalInformation, _subject.AdditionalInformation.AdditionalInformation);
                Assert.Equal(ProjectUrn, _subject.AdditionalInformation.Urn);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage2Performance>(
                        _getInformationForProject.Object, _projectRepository.Object);

                var response = await pageModel.OnGetAsync(ProjectErrorUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }

        public class OnPostAsyncTests : KeyStage2PerformanceTests
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
                await _subject.OnPostAsync(ProjectUrn, string.Empty);

                _projectRepository.Verify(r => r.GetByUrn(ProjectUrn), Times.Once);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel =
                    RazorPageTestHelpers.GetPageModelWithViewData<KeyStage2Performance>(
                        _getInformationForProject.Object, _projectRepository.Object);

                var response = await pageModel.OnPostAsync(ProjectErrorUrn, string.Empty);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInformation = "some additional info";

                var response = await _subject.OnPostAsync(ProjectUrn, additionalInformation);

                var redirectToPageResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal("KeyStage2Performance", redirectToPageResponse.PageName);
                Assert.Equal("OnGetAsync", redirectToPageResponse.PageHandler);
                Assert.Equal(additionalInformation, _foundProject.KeyStage2PerformanceAdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                const string additionalInfo = "test info";

                await _subject.OnPostAsync(ProjectUrn, additionalInfo);
                _projectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.KeyStage2PerformanceAdditionalInformation == additionalInfo
                )));
            }
        }
    }
}