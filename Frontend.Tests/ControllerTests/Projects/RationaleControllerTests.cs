using Data;
using Data.Models;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class RationaleControllerTests
    {
        private readonly RationaleController _subject;
        private readonly Mock<IProjects> _projectRepository;

        public RationaleControllerTests()
        {
            _projectRepository = new Mock<IProjects>();
            _projectRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Project> {Result = new Project()});

            _subject = new RationaleController(_projectRepository.Object);
        }

        public class IndexTests : RationaleControllerTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.Index("0001");

                _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
            }
        }

        public class ProjectTests : RationaleControllerTests
        {
            public class GetTests : ProjectTests
            {
                [Fact]
                public async void GivenUrn_FetchesProjectFromTheRepository()
                {
                    await _subject.Project("0001");

                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }
            }

            public class PostTests : ProjectTests
            {
                [Fact]
                public async void GivenUrnAndRationale_UpdatesTheProject()
                {
                    const string rationale = "This is the project rationale";
                    await _subject.ProjectPost("0001", rationale);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Project == rationale)), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndRationale_RedirectsBackToTheSummary()
                {
                    const string rationale = "This is the project rationale";
                    var result = await _subject.ProjectPost("0001", rationale);

                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }

                [Fact]
                public async void GivenUrnAndNoRationale_AddsErrorToTheModel()
                {
                    var result = await _subject.ProjectPost("0001", "");
                    var model = ControllerTestHelpers.GetViewModelFromResult<RationaleViewModel>(result);
                    Assert.True(model.FormErrors.HasErrors);
                    Assert.True(model.FormErrors.HasErrorForField("rationale"));
                }
            }
        }

        public class TrustOrSponsorTests : RationaleControllerTests
        {
            public class GetTests : TrustOrSponsorTests
            {
                [Fact]
                public async void GivenUrn_FetchesProjectFromTheRepository()
                {
                    await _subject.TrustOrSponsor("0001");

                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }
            }

            public class PostTests : TrustOrSponsorTests
            {
                [Fact]
                public async void GivenUrnAndRationale_UpdatesTheProject()
                {
                    const string rationale = "This is the trust rationale";
                    await _subject.TrustOrSponsorPost("0001", rationale);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Trust == rationale)), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndRationale_RedirectsBackToTheSummary()
                {
                    const string rationale = "This is the project rationale";
                    var result = await _subject.TrustOrSponsorPost("0001", rationale);

                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }

                [Fact]
                public async void GivenUrnAndNoRationale_AddsErrorToTheModel()
                {
                    var result = await _subject.TrustOrSponsorPost("0001", "");
                    var model = ControllerTestHelpers.GetViewModelFromResult<RationaleViewModel>(result);
                    Assert.True(model.FormErrors.HasErrors);
                    Assert.True(model.FormErrors.HasErrorForField("rationale"));
                }
            }
        }
    }
}