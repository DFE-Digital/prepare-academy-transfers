using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Pages.TaskList.KeyStage2Performance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.TaskList.HtbDocument
{
    public class KeyStage2PerformanceTests : BaseTests
    {
        private readonly KeyStage2Performance _subject;

        public KeyStage2PerformanceTests()
        {
            FoundInformationForProject.EducationPerformance = new EducationPerformance
            {
                KeyStage2Performance = new List<KeyStage2>
                {
                    new KeyStage2
                    {
                        Year = "test year"
                    }
                }
            };
            _subject = new KeyStage2Performance(GetInformationForProject.Object, ProjectRepository.Object);
            _subject.Urn = ProjectUrn0001;
            _subject.AdditionalInformationViewModel = new AdditionalInformationViewModel();
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
                const string additionalInformation = "some additional info";
                FoundInformationForProject.Project.KeyStage2PerformanceAdditionalInformation = additionalInformation;
                GetInformationForProject.Setup(s => s.Execute(ProjectUrn0001))
                    .ReturnsAsync(FoundInformationForProject);

                await _subject.OnGetAsync();

                Assert.Equal(additionalInformation, _subject.AdditionalInformationViewModel.AdditionalInformation);
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
            private readonly Project _foundProject;

            public OnPostAsyncTests()
            {
                _foundProject = new Project
                {
                    Urn = ProjectUrn0001
                };

                ProjectRepository.Setup(s => s.GetByUrn(ProjectUrn0001)).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Result = _foundProject
                    });
            }

            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
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
                Assert.Equal(additionalInformation, _foundProject.KeyStage2PerformanceAdditionalInformation);
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
                ProjectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.KeyStage2PerformanceAdditionalInformation == additionalInfo
                )));
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