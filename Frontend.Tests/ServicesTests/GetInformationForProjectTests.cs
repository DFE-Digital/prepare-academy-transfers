using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Services;
using Moq;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class GetInformationForProjectTests
    {
        private readonly GetInformationForProject _subject;
        private readonly Mock<IProjects> _projectsRepository;
        private readonly Mock<IAcademies> _academiesRepository;
        private readonly string _projectUrn;
        private readonly string _academyUkprn;
        private Project _foundProject;
        private Academy _foundAcademy;

        public GetInformationForProjectTests()
        {
            _projectUrn = "projectId";
            _academyUkprn = "1234567";
            _projectsRepository = new Mock<IProjects>();
            _academiesRepository = new Mock<IAcademies>();

            _subject = new GetInformationForProject(_academiesRepository.Object, _projectsRepository.Object);

            SetupRepositories();
        }

        private void SetupRepositories()
        {
            _foundProject = new Project
            {
                Urn = _projectUrn,
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies {OutgoingAcademyUkprn = _academyUkprn}
                }
            };

            _foundAcademy = new Academy
            {
                Ukprn = _academyUkprn
            };

            _projectsRepository.Setup(r => r.GetByUrn(_projectUrn)).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = _foundProject
                });

            _academiesRepository.Setup(r => r.GetAcademyByUkprn(_academyUkprn)).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Result = _foundAcademy
                });
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectInRepository()
        {
            await _subject.Execute(_projectUrn);

            _projectsRepository.Verify(r => r.GetByUrn(_projectUrn), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectAcademyInDynamics()
        {
            await _subject.Execute(_projectUrn);

            _academiesRepository.Verify(r => r.GetAcademyByUkprn(_academyUkprn), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_ReturnsFoundProjectAndAcademy()
        {
            var result = await _subject.Execute(_projectUrn);

            Assert.Equal(result.Project, _foundProject);
            Assert.Equal(result.OutgoingAcademy, _foundAcademy);
        }
    }
}