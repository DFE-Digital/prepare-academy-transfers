using System.Collections.Generic;
using Data;
using Data.Models;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Pages.Projects.LatestOfstedJudgement;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.LatestOfstedJudgement
{
    public class IndexTests : PageTests
    {
        private readonly Index _subject;

        public IndexTests()
        {
            _subject = new Index(GetInformationForProject.Object, ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }
        
        public class OnGetAsyncTests : IndexTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync();
            
                GetInformationForProject.Verify(r => r.Execute(ProjectUrn0001), Times.Once);
            }
            
            [Fact]
            public async void GivenExecuteReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;
                var response = await _subject.OnGetAsync();
            
                var viewResult = Assert.IsType<ViewResult>(response);
                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
       
            [Fact]
            public async void GivenUrn_AssignsModelToThePage()
            {
                var response = await _subject.OnGetAsync();
            
                Assert.IsType<PageResult>(response);
                Assert.Equal(ProjectUrn0001,_subject.Urn);
                Assert.Equal(AcademyUrn,_subject.OutgoingAcademyUrn);
            }
        }

        public class OnPostAsyncTests : IndexTests
        {
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProject()
            {
                var additionalInformation = "some additional information";
                _subject.AdditionalInformationViewModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = additionalInformation
                };

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.Update(It.Is<Data.Models.Project>(project => 
                            project.LatestOfstedJudgementAdditionalInformation == additionalInformation)),
                    Times.Once);
            }
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                var additionalInformation = "some additional information";
                _subject.AdditionalInformationViewModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = additionalInformation
                };

                var response = await _subject.OnPostAsync();
                
                var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Urn", ProjectUrn0001)
                });
                ControllerTestHelpers.AssertResultRedirectsToPage(response, $"/Projects/LatestOfstedJudgement/{nameof(Index)}", routeValues);
                Assert.Equal(additionalInformation, FoundProjectFromRepo.LatestOfstedJudgementAdditionalInformation);
            }
            
            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;
                var response = await _subject.OnPostAsync();
                var viewResult = Assert.IsType<ViewResult>(response);
            
                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
            
            [Fact]
            public async void GivenUpdateReturnsError_DisplayErrorPage()
            {
                ProjectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                    .ReturnsAsync(new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            StatusCode = System.Net.HttpStatusCode.NotFound,
                            ErrorMessage = "Project not found"
                        }
                    });
                
                var response = await _subject.OnPostAsync();
                var viewResult = Assert.IsType<ViewResult>(response);
            
                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Project not found", viewResult.Model);
            }
            
            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;
                var response = await _subject.OnPostAsync();
                
                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response,
                    Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {id = ProjectUrn0001})
                );
            }
        }
    }
}