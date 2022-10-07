using Data.Models.Projects;
using Frontend.Pages.Projects.LegalRequirements;
using Frontend.Tests.Helpers;
using Frontend.Tests.PagesTests.Projects.LegalRequirements;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Frontend.Models;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.LegalRequirements
{
    public class FoundationConsentTests : BaseTests
    {
        private readonly FoundationConsentModel _subject;

        public FoundationConsentTests()
        {
            _subject = new FoundationConsentModel(ProjectRepository.Object);
        }

        public class GetTests : FoundationConsentTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                _subject.Urn = ProjectUrn0001;
                await _subject.OnGetAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async Task GivenReturnToPreview_KeepsValue()
            {
                _subject.ReturnToPreview = true;

                await _subject.OnGetAsync();

                Assert.True(_subject.ReturnToPreview);
            }
        }

        public class PostTests : FoundationConsentTests
        {
            [Fact]
            public async void GivenUrnAndFoundationConsent_UpdatesTheProject()
            {

                _subject.FoundationConsentViewModel.FoundationConsent = ThreeOptions.No;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(
                        project => project.LegalRequirements.FoundationConsent == ThreeOptions.No)));
            }


            [Fact]
            public async Task GivenUrnAndFoundationConsent_RedirectsToTheSummaryPage()
            {
                _subject.Urn = ProjectUrn0001;

                _subject.FoundationConsentViewModel.FoundationConsent = ThreeOptions.No;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/LegalRequirements/Index",
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }

            [Fact]
            public async Task GivenReturnToPreview_RedirectToThePreviewPage()
            {
                _subject.FoundationConsentViewModel.FoundationConsent = ThreeOptions.No;
                _subject.Urn = ProjectUrn0001;
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }
        }
    }
}