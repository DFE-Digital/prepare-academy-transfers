﻿using Dfe.PrepareTransfers.Web.Pages.Projects.LegalRequirements;
using Dfe.PrepareTransfers.Web.Tests.Dfe.PrepareTransfers.Helpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Models;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.LegalRequirements
{
    public class OutgoingTrustConsentTests : BaseTests
    {
        private readonly OutgoingTrustConsentModel _subject;

        public OutgoingTrustConsentTests()
        {
            _subject = new OutgoingTrustConsentModel(ProjectRepository.Object);
        }

        public class GetTests : OutgoingTrustConsentTests
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

        public class PostTests : OutgoingTrustConsentTests
        {
            [Fact]
            public async Task GivenUrnAndOutgoingTrustConsent_UpdatesTheProject()
            {
                _subject.OutgoingTrustConsentViewModel.OutgoingTrustConsent = ThreeOptions.No;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.UpdateLegalRequirements(It.Is<Project>(
                        project => project.LegalRequirements.OutgoingTrustConsent == ThreeOptions.No)));
            }


            [Fact]
            public async Task GivenUrnAndOutgoingTrustConsent_RedirectsToTheSummaryPage()
            {
                _subject.Urn = ProjectUrn0001;

                _subject.OutgoingTrustConsentViewModel.OutgoingTrustConsent = ThreeOptions.No;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/LegalRequirements/Index",
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }

            [Fact]
            public async Task GivenReturnToPreview_RedirectToThePreviewPage()
            {
                _subject.OutgoingTrustConsentViewModel.OutgoingTrustConsent = ThreeOptions.No;
                _subject.Urn = ProjectUrn0001;
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }
        }
    }
}
