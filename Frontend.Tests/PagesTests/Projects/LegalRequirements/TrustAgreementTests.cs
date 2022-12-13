﻿using Data.Models.Projects;
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
    public class TrustAgreementTests : BaseTests
    {
        private readonly IncomingTrustAgreementModel _subject;

        public TrustAgreementTests()
        {
            _subject = new IncomingTrustAgreementModel(ProjectRepository.Object);
        }

        public class GetTests : TrustAgreementTests
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

        public class PostTests : TrustAgreementTests
        {
            [Fact]
            public async Task GivenUrnAndTrustAgreement_UpdatesTheProject()
            {
                _subject.IncomingTrustAgreementViewModel.IncomingTrustAgreement = ThreeOptions.No;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(
                        project => project.LegalRequirements.IncomingTrustAgreement == ThreeOptions.No)));
            }


            [Fact]
            public async Task GivenUrnAndTrustAgreement_RedirectsToTheSummaryPage()
            {
                _subject.Urn = ProjectUrn0001;
                
                _subject.IncomingTrustAgreementViewModel.IncomingTrustAgreement = ThreeOptions.No;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/LegalRequirements/Index",
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }

            [Fact]
            public async Task GivenReturnToPreview_RedirectToThePreviewPage()
            {
                _subject.IncomingTrustAgreementViewModel.IncomingTrustAgreement = ThreeOptions.No;
                _subject.Urn = ProjectUrn0001;
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 }));
            }
        }
    }
}