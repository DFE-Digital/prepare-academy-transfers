using Data.Models;
using Data.Models.Academies;
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
        public class OnGetAsyncTests : KeyStage2PerformanceTests
        {
            private const string ProjectErrorUrn = "errorUrn";
            private const string ProjectUrn = "0001";
            private const string AcademyUrn = "1234";
            private const string AcademyName = "Academy Name";
            private const string LAName = "LA Name";
            private readonly Mock<IGetInformationForProject> _getInformationForProject;
            private readonly GetInformationForProjectResponse _foundInformationForProject;
            private readonly KeyStage2Performance _subject;

            public OnGetAsyncTests()
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
                        Name = AcademyName,
                        Performance = new AcademyPerformance()
                    }
                };
                _subject = new KeyStage2Performance(_getInformationForProject.Object);
                
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
                Assert.Equal(_subject.ProjectUrn, ProjectUrn);
                Assert.Equal(_subject.OutgoingAcademyUrn, AcademyUrn);
                Assert.Equal(_subject.OutgoingAcademyName, AcademyName);
                Assert.Equal(_subject.LocalAuthorityName, LAName);
            }
            
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                const string additionalInformation = "some additional info";
                _foundInformationForProject.Project.KeyStage2PerformanceAdditionalInformation = additionalInformation;
                _getInformationForProject.Setup(s => s.Execute(ProjectUrn))
                    .ReturnsAsync(_foundInformationForProject);

                var response = await _subject.OnGetAsync(ProjectUrn);
                
                Assert.Equal(additionalInformation, _subject.AdditionalInformation.AdditionalInformation);
                Assert.Equal(ProjectUrn, _subject.AdditionalInformation.Urn);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var pageModel = RazorPageTestHelpers.GetPageModelWithViewData<KeyStage2Performance>(_getInformationForProject.Object);

                var response = await pageModel.OnGetAsync(ProjectErrorUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }
    }
}