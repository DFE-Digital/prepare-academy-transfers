using Data.Models;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.Features
{
    public class ReasonTests : BaseTests
    {
        private readonly Pages.Projects.Features.Reason _subject;

        public ReasonTests()
        {
            _subject = new Pages.Projects.Features.Reason(ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }

        public class ReasonGetTests : ReasonTests
        {
            [Fact]
            public async void GivenUrn_GetsProject()
            {
                await _subject.OnGetAsync();
                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }
        }

        public class ReasonPostTests : ReasonTests
        {
            [Fact]
            public async void GivenSubjectToInterventionAndReason_UpdatesTheProject()
            {
                _subject.FeaturesReasonViewModel.IsSubjectToIntervention = true;
                _subject.FeaturesReasonViewModel.MoreDetail = "More detail";

                await _subject.OnPostAsync();
                ProjectRepository.Verify(r => r.Update(It.Is<Project>(project =>
                    project.Urn == ProjectUrn0001 &&
                    project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true &&
                    project.Features.ReasonForTransfer.InterventionDetails == "More detail")), Times.Once);
            }

            [Fact]
            public async void GivenSubjectToInterventionAndReason_RedirectsToSummaryPage()
            {
                _subject.FeaturesReasonViewModel.IsSubjectToIntervention = true;
                _subject.FeaturesReasonViewModel.MoreDetail = "More detail";

                var result = await _subject.OnPostAsync();
                ControllerTestHelpers.AssertResultRedirectsToPage(result, "/Projects/Features/Index",
                    new RouteValueDictionary(new {Urn = ProjectUrn0001}));
            }

            [Fact]
            public async void GivenNotSubjectToInterventionAndNoReason_UpdatesTheProject()
            {
                _subject.FeaturesReasonViewModel.IsSubjectToIntervention = false;
                await _subject.OnPostAsync();
                ProjectRepository.Verify(r => r.Update(It.Is<Project>(project =>
                    project.Urn == ProjectUrn0001 &&
                    project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == false &&
                    project.Features.ReasonForTransfer.InterventionDetails == string.Empty)), Times.Once);
            }

            [Fact]
            public async void GivenNothingSubmitted_DoesNotUpdateTheProject()
            {
                await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(),
                    _subject.FeaturesReasonViewModel, _subject.ModelState);
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
            }

            [Fact]
            public async void GivenNothingSubmitted_PageResultReturned()
            {
                await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(),
                    _subject.FeaturesReasonViewModel, _subject.ModelState);
                var resultPost = await _subject.OnPostAsync();

                Assert.IsType<PageResult>(resultPost);
                Assert.False(_subject.ModelState.IsValid);
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {id = "0001"})
                );
            }
        }
    }
}