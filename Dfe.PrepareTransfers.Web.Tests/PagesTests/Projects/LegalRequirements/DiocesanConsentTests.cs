using Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Pages.Projects.LegalRequirements;
using Dfe.PrepareTransfers.Web.Tests.Helpers;
using Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.LegalRequirements;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Dfe.PrepareTransfers.Web.Models;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.LegalRequirements
{
    public class DiocesanConsentTests : BaseTests
    {
        private readonly DiocesanConsentModel _subject;

        public DiocesanConsentTests()
        {
            _subject = new DiocesanConsentModel(ProjectRepository.Object);
        }

        public class GetTests : DiocesanConsentTests
        {
            [Fact]
            public async Task GivenUrn_FetchesProjectFromTheRepository()
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

        public class PostTests : DiocesanConsentTests
        {
            [Fact]
            public async Task GivenUrnAndDiocesanConsent_UpdatesTheProject()
            {
                _subject.DiocesanConsentViewModel.DiocesanConsent = ThreeOptions.No;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(
                        project => project.LegalRequirements.DiocesanConsent == ThreeOptions.No)));
            }


            [Fact]
            public async Task GivenUrnAndDiocesanConsent_RedirectsToTheSummaryPage()
            {
                _subject.Urn = ProjectUrn0001;

                _subject.DiocesanConsentViewModel.DiocesanConsent = ThreeOptions.No;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/LegalRequirements/Index",
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }

            [Fact]
            public async Task GivenReturnToPreview_RedirectToThePreviewPage()
            {
                _subject.DiocesanConsentViewModel.DiocesanConsent = ThreeOptions.No;
                _subject.Urn = ProjectUrn0001;
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }
        }
    }
}
