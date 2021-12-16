using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.AcademyAndTrustInformation;
using Frontend.Pages.Projects.AcademyAndTrustInformation;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.AcademyAndTrustInformation
{
    public class RecommendationTests : PageTests
    {
        protected readonly Recommendation Subject;

        protected RecommendationTests()
        {
            Subject = new Recommendation(ProjectRepository.Object);
        }
    }
    
    public class RecommendationGetTests : RecommendationTests
    {
        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            await Subject.OnGetAsync(ProjectUrn);
        
            ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn), Times.Once);
        }
        
        [Fact]
        public async void GivenReturnToPreview_AssignItToTheView()
        {
            var response = await Subject.OnGetAsync(ProjectUrn, true);
            
            Assert.IsType<PageResult>(response);
            Assert.True(Subject.ReturnToPreview);
        }
        
        [Fact]
        public async void GivenGetByUrnReturnsError_DisplayErrorPage()
        {
            var response =  await Subject.OnGetAsync(ProjectErrorUrn);
            
            var viewResult = Assert.IsType<ViewResult>(response);
            Assert.Equal("ErrorPage", viewResult.ViewName);
            Assert.Equal("Error", viewResult.Model);
        }
    }
    
    public class RecommendationPostTests : RecommendationTests
    {
        private readonly RecommendationViewModel _vm;
        public RecommendationPostTests()
        {
            _vm = new RecommendationViewModel
            {
                Urn = ProjectUrn,
                Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve,
                Author = "Author"
            };
        }
        [Fact]
        public async void GivenUrnAndRecommendationAndAuthor_UpdatesTheProject()
        {
            await Subject.OnPostAsync(_vm);
        
            ProjectRepository.Verify(r =>
                r.Update(It.Is<Project>(project =>
                    project.AcademyAndTrustInformation.Recommendation == _vm.Recommendation &&
                    project.AcademyAndTrustInformation.Author == _vm.Author)), Times.Once);
        }
        
        [Fact]
        public async void GivenUrnAndRecommendationAndAuthor_RedirectsBackToTheSummary()
        {
            var result = await Subject.OnPostAsync(_vm);
        
            var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Urn", ProjectUrn)
            });
            ControllerTestHelpers.AssertResultRedirectsToPage(result, $"/Projects/AcademyAndTrustInformation/{nameof(Index)}", routeValues);
        }
        
        [Fact]
        public async void GivenGetByUrnReturnsError_DisplayErrorPage()
        {
            var vmError = new RecommendationViewModel
            {
                Urn = ProjectErrorUrn,
                Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Decline
            };
            var response = await Subject.OnPostAsync(vmError);
            
            var viewResult = Assert.IsType<ViewResult>(response);
            Assert.Equal("ErrorPage", viewResult.ViewName);
            Assert.Equal("Error", viewResult.Model);
        }
        
        [Fact]
        public async void GivenUpdateReturnsError_DisplayErrorPage()
        {
            ProjectRepository.Setup(s => s.Update(It.IsAny<Project>()))
                .ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "Update error"
                        }
                    });

            var response = await Subject.OnPostAsync(_vm);
            
            var viewResult = Assert.IsType<ViewResult>(response);
            Assert.Equal("ErrorPage", viewResult.ViewName);
            Assert.Equal("Update error", viewResult.Model);
        }
        
        [Fact]
        public async void GivenReturnToPreview_ReturnToThePreviewPage()
        {
            _vm.ReturnToPreview = true;
            
            var response = await Subject.OnPostAsync(_vm);
            
            ControllerTestHelpers.AssertResultRedirectsToPage(response,
                Links.HeadteacherBoard.Preview.PageName, new RouteValueDictionary(new {id = ProjectUrn}));
        }
    }
}