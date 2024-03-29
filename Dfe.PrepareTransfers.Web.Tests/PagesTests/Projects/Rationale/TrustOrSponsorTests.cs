using System.Collections.Generic;
using AutoFixture;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Rationale;
using Dfe.PrepareTransfers.Web.Pages.Projects.Rationale;
using Dfe.PrepareTransfers.Web.Tests.Dfe.PrepareTransfers.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.Rationale
{
    public class TrustOrSponsorTests : BaseTests
    {
        private readonly TrustOrSponsor _subject;

        public TrustOrSponsorTests()
        {
            _subject = new TrustOrSponsor(ProjectRepository.Object) { Urn = ProjectUrn0001 };
        }
        
        public class OnGetAsync : TrustOrSponsorTests
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
                var fixture = new Fixture();
                var foundProject = fixture.Build<Data.Models.Project>()
                    .With(project => project.Urn, ProjectUrn0001).Create();

                ProjectRepository.Setup(s => s.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                    new RepositoryResult<Data.Models.Project>
                    {
                        Result = foundProject
                    });

                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(foundProject.Urn, _subject.Urn);
                Assert.Equal(foundProject.Rationale.Trust, _subject.ViewModel.TrustOrSponsorRationale);
            }
        }

        public class OnPostAsync : TrustOrSponsorTests
        {
            public OnPostAsync()
            {
                _subject.Urn = ProjectUrn0001;
                _subject.ViewModel = new RationaleTrustOrSponsorViewModel()
                {
                    TrustOrSponsorRationale = "This is the trust rationale"
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
                _subject.ModelState.AddModelError(nameof(_subject.ViewModel.TrustOrSponsorRationale), "error");
                var result = await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.UpdateRationale(It.Is<Data.Models.Project>(project => project.Urn == ProjectUrn0001)), Times.Never);
                
                Assert.IsType<PageResult>(result);
            }

            [Fact]
            public async void GivenUrnAndRationale_UpdatesTheProject()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.UpdateRationale(It.Is<Data.Models.Project>(project => project.Rationale.Trust == _subject.ViewModel.TrustOrSponsorRationale)),
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
                const string rationale = "This is the trust rationale";
                _subject.ViewModel = new RationaleTrustOrSponsorViewModel()
                {
                    TrustOrSponsorRationale = rationale
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