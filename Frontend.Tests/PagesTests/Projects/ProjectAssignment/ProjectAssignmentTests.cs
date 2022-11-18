using AutoFixture;
using Data;
using Data.Models;
using Frontend.Models;
using Frontend.Pages.Projects.ProjectAssignment;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.ProjectAssignment
{
    public class ProjectAssignmentTests
    {
        public class GetTests : ProjectAssignmentTests
        {
            private readonly IndexModel _subject;
            private readonly Mock<IUserRepository> _userRepository;
            private readonly Mock<IProjects> _projectRepository;
            private readonly Fixture _fixture = new Fixture();

            public GetTests()
            {
                _userRepository = new Mock<IUserRepository>();
                _projectRepository = new Mock<IProjects>();
                _subject = new IndexModel(_userRepository.Object, _projectRepository.Object);
            }

            [Fact]
            public async Task Should_get_deliveryofficers()
            {
                var deliveryOfficers = _fixture.CreateMany<User>();
                _userRepository.Setup(m => m.GetAllUsers()).ReturnsAsync(deliveryOfficers);

                _projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(_fixture.Create<RepositoryResult<Project>>());

                var result = await _subject.OnGetAsync("12345");

                Assert.Multiple(
                   () => Assert.IsType<PageResult>(result),
                   () => Assert.Equivalent(deliveryOfficers, _subject.DeliveryOfficers));
            }

            [Fact]
            public async Task Should_get_project_fields()
            {
                var project = _fixture.Create<RepositoryResult<Project>>();
                _projectRepository.Setup(m => m.GetByUrn(It.IsAny<string>())).ReturnsAsync(project);

                var urn = "12345";
                await _subject.OnGetAsync(urn);

                Assert.Multiple(
                    () => Assert.Equal(urn, _subject.Urn),
                    () => Assert.Equal(project.Result.IncomingTrustName, _subject.IncomingTrustName),
                    () => Assert.Equal(project.Result.AssignedUser.FullName, _subject.SelectedDeliveryOfficer)
                );
            }
        }
    }
}
