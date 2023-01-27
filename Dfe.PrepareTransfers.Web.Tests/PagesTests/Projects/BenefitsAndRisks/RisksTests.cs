using System;
using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks;
using Dfe.PrepareTransfers.Web.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.BenefitsAndRisks
{
    public class RisksTests : BaseTests
    {
        private readonly Risks _subject;

        public RisksTests()
        {
            _subject = new Risks(ProjectRepository.Object);
        }

        public class PostTests : RisksTests
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async void GivenUrnAndYesOrNo_UpdateTheProject(bool yesNo)
            {
                _subject.RisksViewModel.RisksInvolved = yesNo;
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(project => project.Benefits.AnyRisks == yesNo))
                );
            }

            [Fact]
            public async void GivenUrnAndNo_RedirectsToTheSummaryPage()
            {
                _subject.RisksViewModel.RisksInvolved = false;
                _subject.Urn = ProjectUrn0001;

                var resp = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(resp, "/Projects/BenefitsAndRisks/Index",
                    new RouteValueDictionary(new {Urn = ProjectUrn0001}));
            }
            
            
            [Fact]
            public async void GivenUrnAndYes_RedirectsToOtherFactorsPage()
            {
                _subject.RisksViewModel.RisksInvolved = true;
                _subject.Urn = ProjectUrn0001;

                var resp = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(resp, "/Projects/BenefitsAndRisks/OtherFactors",
                    new RouteValueDictionary(new {Urn = ProjectUrn0001}));
            }

            [Fact]
            //Only go back to preview if No, Yes will take them through the options
            public async void GivenReturnToPreviewAndNo_RedirectToThePreviewPage()
            {
                _subject.RisksViewModel.RisksInvolved = true;
                _subject.Urn = ProjectUrn0001;
                _subject.RisksViewModel.RisksInvolved = false;
                _subject.ReturnToPreview = true;
                var resp = await _subject.OnPostAsync();

                var result = await _subject.OnPostAsync();
                ControllerTestHelpers.AssertResultRedirectsToPage(resp, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {Urn = ProjectUrn0001}));
            }
        }
    }
}