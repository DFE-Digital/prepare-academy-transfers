using Data;
using Data.Models;
using Frontend.Controllers.Projects;
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
        }
    }
}