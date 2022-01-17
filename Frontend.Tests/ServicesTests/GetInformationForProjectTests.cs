using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
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
        private readonly Mock<IEducationPerformance> _educationPerformanceRepository;
        private readonly string _projectUrn;
        private readonly string _academyUkprn;
        private readonly string _academyUrn;
        private Project _foundProject;
        private Academy _foundAcademy;
        private EducationPerformance _foundEducationPerformance;

        public GetInformationForProjectTests()
        {
            _projectUrn = "projectId";
            _academyUkprn = "1234567";
            _academyUrn = "1234567";
            _projectsRepository = new Mock<IProjects>();
            _academiesRepository = new Mock<IAcademies>();
            _educationPerformanceRepository = new Mock<IEducationPerformance>();

            _subject = new GetInformationForProject(
                _academiesRepository.Object, 
                _projectsRepository.Object, 
                _educationPerformanceRepository.Object);

            SetupRepositories();
        }

        private void SetupRepositories()
        {
            _foundProject = new Project
            {
                Urn = _projectUrn,
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = _academyUkprn, 
                        OutgoingAcademyUrn = _academyUrn
                    }
                }
            };

            _foundAcademy = new Academy
            {
                Ukprn = _academyUkprn,
                Urn = _academyUrn
            };

            _foundEducationPerformance = new EducationPerformance
            {
                KeyStage2Performance = new List<KeyStage2>()
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

            _educationPerformanceRepository.Setup(r => r.GetByAcademyUrn(_academyUrn))
                .ReturnsAsync(
                    new RepositoryResult<EducationPerformance>
                    {
                        Result = _foundEducationPerformance
                    });
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectInRepository()
        {
            await _subject.Execute(_projectUrn);

            _projectsRepository.Verify(r => r.GetByUrn(_projectUrn), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectAcademy()
        {
            await _subject.Execute(_projectUrn);

            _academiesRepository.Verify(r => r.GetAcademyByUkprn(_academyUkprn), Times.Once);
        }
        
        [Fact]
        public async void GivenProjectId_LooksUpProjectAcademyEducationPerformance()
        {
            await _subject.Execute(_projectUrn);

            _educationPerformanceRepository.Verify(r => r.GetByAcademyUrn(_academyUrn), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_ReturnsFoundProjectInformation()
        {
            var result = await _subject.Execute(_projectUrn);

            Assert.Equal(result.Project, _foundProject);
            Assert.Equal(result.OutgoingAcademy, _foundAcademy);
            Assert.Equal(result.EducationPerformance, _foundEducationPerformance);
        }
    }
}