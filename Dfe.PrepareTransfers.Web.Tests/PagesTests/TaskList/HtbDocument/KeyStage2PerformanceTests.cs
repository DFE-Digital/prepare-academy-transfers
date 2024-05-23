using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Pages.TaskList.KeyStage2Performance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.TaskList.HtbDocument
{
    public class KeyStage2PerformanceTests : BaseTests
    {
        private readonly KeyStage2Performance _subject;

        public KeyStage2PerformanceTests()
        {
            FoundInformationForProject.OutgoingAcademies.First().EducationPerformance = new EducationPerformance
            {
                KeyStage2Performance = new List<KeyStage2>
                {
                    new KeyStage2
                    {
                        Year = "test year"
                    }
                },
                KeyStage2AdditionalInformation = "some additional info"
            };
            _subject = new KeyStage2Performance(GetInformationForProject.Object, ProjectRepository.Object)
            {
                Urn = ProjectUrn0001,
                AcademyUkprn = AcademyUkprn,
                AdditionalInformationViewModel = new AdditionalInformationViewModel()
            };
        }

        public class OnGetAsyncTests : KeyStage2PerformanceTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync();

                GetInformationForProject.Verify(r => r.Execute(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenExistingProject_AssignsItToThePageModel()
            {
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(ProjectUrn0001, _subject.Urn);
                Assert.Equal(AcademyUrn, _subject.OutgoingAcademyUrn);
                Assert.Equal("test year", _subject.EducationPerformance.KeyStage2Performance[0].Year);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                GetInformationForProject.Setup(s => s.Execute(ProjectUrn0001))
                    .ReturnsAsync(FoundInformationForProject);
                
                await _subject.OnGetAsync();

                Assert.Equal(FoundInformationForProject.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn).EducationPerformance.KeyStage2AdditionalInformation, _subject.AdditionalInformationViewModel.AdditionalInformation);
                Assert.Equal(ProjectUrn0001, _subject.AdditionalInformationViewModel.Urn);
            }
            
            [Fact]
            public async void GivenReturnToPreview_UpdatesTheViewModel()
            {
                _subject.ReturnToPreview = true;
                await _subject.OnGetAsync();

                Assert.True(_subject.ReturnToPreview);
            }
        }

        public class OnPostAsyncTests : KeyStage2PerformanceTests
        {
            [Fact]
            public async void GivenAcademyUkprn_FetchesAcademyFromTheRepository()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInformation = "some additional info";
                _subject.AdditionalInformationViewModel = new AdditionalInformationViewModel()
                {
                    AdditionalInformation = additionalInformation,
                    AddOrEditAdditionalInformation = true

                };
                var response = await _subject.OnPostAsync();

                var redirectToPageResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal("KeyStage2Performance", redirectToPageResponse.PageName);
                Assert.Null(redirectToPageResponse.PageHandler);
                Assert.Equal(additionalInformation, FoundProjectFromRepo.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn).KeyStage2PerformanceAdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                const string additionalInfo = "test info";
                _subject.AdditionalInformationViewModel = new AdditionalInformationViewModel()
                {
                    AdditionalInformation = additionalInfo,
                    AddOrEditAdditionalInformation = true
                };
                await _subject.OnPostAsync();
                ProjectRepository.Verify(r => r.UpdateAcademy(It.Is<string>(x => x == _subject.Urn), It.Is<TransferringAcademy>(academy => academy.OutgoingAcademyUkprn == _subject.AcademyUkprn && academy.KeyStage2PerformanceAdditionalInformation == additionalInfo)
                ));
            }
            
            [Fact]
            public async void GivenReturnToPreview_RedirectsToThePreviewPage()
            {
                _subject.ReturnToPreview = true;
                var response = await _subject.OnPostAsync();

                var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal(Links.HeadteacherBoard.Preview.PageName, redirectResponse.PageName);
                Assert.Equal(ProjectUrn0001, redirectResponse.RouteValues["urn"]);
            }
        }
    }
}