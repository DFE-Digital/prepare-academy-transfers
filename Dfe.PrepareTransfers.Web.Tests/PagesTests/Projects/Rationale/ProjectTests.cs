using System.Collections.Generic;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Rationale;
using Dfe.PrepareTransfers.Web.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using Dfe.PrepareTransfers.Web.Pages.Projects.Rationale;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.Rationale
{
    public class ProjectTests : BaseTests
    {
        private readonly Project _subject;

        protected ProjectTests()
        {
            _subject = new Project(ProjectRepository.Object) { Urn = ProjectUrn0001 };
        }

        public class OnGetAsync : ProjectTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenReturnToPreview_AssignItToThePage()
            {
                _subject.ReturnToPreview = true;
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.True(_subject.ReturnToPreview);
            }

            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToThePageModel()
            {
                _subject.Urn = PopulatedProjectUrn;
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(FoundPopulatedProjectFromRepo.Urn, _subject.Urn);
                Assert.Equal(FoundPopulatedProjectFromRepo.Rationale.Project, _subject.ViewModel.ProjectRationale);
            }
        }

        public class OnPostAsync : ProjectTests
        {
            public OnPostAsync()
            {
                _subject.Urn = ProjectUrn0001;
                _subject.ViewModel = new RationaleProjectViewModel
                {
                    ProjectRationale = "This is the project rationale"
                };
            }

            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenErrorInModelState_ReturnsCorrectPage()
            {
                _subject.ModelState.AddModelError(nameof(_subject.ViewModel.ProjectRationale), "error");
                var result = await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Data.Models.Project>(project => project.Urn == ProjectUrn0001)), Times.Never);
                
                Assert.IsType<PageResult>(result);
            }

            [Fact]
            public async void GivenUrnAndRationale_UpdatesTheProject()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.Update(It.Is<Data.Models.Project>(project => project.Rationale.Project == _subject.ViewModel.ProjectRationale)),
                    Times.Once);
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();
    
                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {Urn = ProjectUrn0001})
                );
            }
            
            [Fact]
            public async void GivenUrnAndRationale_RedirectsBackToTheSummary()
            {
                const string rationale = "This is the project rationale";
                _subject.ViewModel = new RationaleProjectViewModel
                {
                    ProjectRationale = rationale
                };

                var result = await _subject.OnPostAsync();

                var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Urn", ProjectUrn0001)
                });
                ControllerTestHelpers.AssertResultRedirectsToPage(result, $"/Projects/Rationale/{nameof(Index)}", routeValues);
            }
        }
    }
}