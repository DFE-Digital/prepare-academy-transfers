using Data.Models;
using Frontend.Pages;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests
{
    public class KeyStage2PerformanceTests
    {
        public class OnGetAsyncTests : KeyStage2PerformanceTests
        {
            private const string ProjectErrorUrn = "errorUrn";
            private const string ProjectUrn = "0001";
            private const string AcademyUrn = "1234";
            private readonly Mock<IGetInformationForProject> _getInformationForProject;
            private readonly KeyStage2Performance _subject;

            public OnGetAsyncTests()
            {
                _getInformationForProject = new Mock<IGetInformationForProject>();
                _subject = new KeyStage2Performance(_getInformationForProject.Object);
                
                _getInformationForProject.Setup(s => s.Execute(ProjectUrn))
                    .ReturnsAsync(
                        new GetInformationForProjectResponse
                        {
                            Project = new Project
                            {
                                Urn = ProjectUrn
                            },
                            OutgoingAcademy = new Academy
                            {
                                Urn = AcademyUrn
                            }
                        });

                _getInformationForProject.Setup(s => s.Execute(ProjectErrorUrn))
                    .ReturnsAsync(
                        new GetInformationForProjectResponse
                        {
                            ResponseError = new ServiceResponseError
                            {
                                ErrorMessage = "Error"
                            }
                        });
            }
            
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
                Assert.Equal(_subject.Project.Urn, ProjectUrn);
                Assert.Equal(_subject.TransferringAcademy.Urn, AcademyUrn);
            }
            
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInformation = "some additional info";
                _getInformationForProject.Setup(s => s.Execute(ProjectUrn))
                    .ReturnsAsync(
                        new GetInformationForProjectResponse
                        {
                            Project = new Project
                            {
                                Urn = ProjectUrn,
                                KeyStage2PerformanceAdditionalInformation = additionalInformation
                            }
                        });

                var response = await _subject.OnGetAsync(ProjectUrn);
                
                Assert.Equal(additionalInformation, _subject.AdditionalInformation.AdditionalInformation);
                Assert.Equal(ProjectUrn, _subject.AdditionalInformation.Urn);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var response = await _subject.OnGetAsync(ProjectErrorUrn);
                var redirectToPageResult = Assert.IsType<RedirectToPageResult>(response);

                Assert.Equal("ErrorPage", redirectToPageResult.PageName);
                Assert.Equal("Error", redirectToPageResult.PageHandler);
            }
        }
    }
}