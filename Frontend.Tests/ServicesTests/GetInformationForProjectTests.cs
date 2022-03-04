using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Frontend.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
        private Mock<IDistributedCache> _distributedCache;

        public GetInformationForProjectTests()
        {
            _projectUrn = "projectId";
            _academyUkprn = "1234567";
            _academyUrn = "1234567";
            _projectsRepository = new Mock<IProjects>();
            _academiesRepository = new Mock<IAcademies>();
            _educationPerformanceRepository = new Mock<IEducationPerformance>();
            _distributedCache = new Mock<IDistributedCache>();

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
                        OutgoingAcademyUrn = _academyUrn,
                        IncomingTrustName = "incoming trust name"
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
        }

        [Fact]
        public async void GivenProjectWithMultipleAcademies_LooksUpAcademies()
        {
            const string projectUrn = "TestProject";
            const string incomingTrustName = "Incoming Trust Name";
            var outgoingAcademy1 = new { ukprn = "A1Ukprn", urn = "A1Urn" };
            var outgoingAcademy2 = new { ukprn = "A2Ukprn", urn = "A2Urn" };
            var outgoingAcademy3 = new { ukprn = "A3Ukprn", urn = "A3Urn" };
            var foundProjectWithMultipleAcademies = new Project
            {
                Urn = projectUrn,
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = outgoingAcademy1.ukprn,
                        OutgoingAcademyUrn = outgoingAcademy1.urn,
                        IncomingTrustName = incomingTrustName
                    },
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = outgoingAcademy2.ukprn,
                        OutgoingAcademyUrn = outgoingAcademy2.urn,
                        IncomingTrustName = incomingTrustName
                    },
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = outgoingAcademy3.ukprn,
                        OutgoingAcademyUrn = outgoingAcademy3.urn,
                        IncomingTrustName = incomingTrustName
                    }
                }
            };

            var foundAcademy1 = new Academy { Ukprn = outgoingAcademy1.ukprn, Urn = outgoingAcademy1.urn };
            var foundAcademy2 = new Academy { Ukprn = outgoingAcademy2.ukprn, Urn = outgoingAcademy2.urn };
            var foundAcademy3 = new Academy { Ukprn = outgoingAcademy3.ukprn, Urn = outgoingAcademy3.urn };

            _projectsRepository.Setup(r => r.GetByUrn(projectUrn)).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = foundProjectWithMultipleAcademies
                });

            _academiesRepository.Setup(r => r.GetAcademyByUkprn(outgoingAcademy1.ukprn)).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Result = foundAcademy1
                });
            
            _academiesRepository.Setup(r => r.GetAcademyByUkprn(outgoingAcademy2.ukprn)).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Result = foundAcademy2
                });
            
            _academiesRepository.Setup(r => r.GetAcademyByUkprn(outgoingAcademy3.ukprn)).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Result = foundAcademy3
                });

            _educationPerformanceRepository.Setup(r => r.GetByAcademyUrn(It.IsAny<string>()))
                .ReturnsAsync(
                    new RepositoryResult<EducationPerformance>
                    {
                        Result = _foundEducationPerformance
                    });
            
            var result = await _subject.Execute(projectUrn);

            _academiesRepository.Verify(r => r.GetAcademyByUkprn(outgoingAcademy1.ukprn), Times.Once);
            _academiesRepository.Verify(r => r.GetAcademyByUkprn(outgoingAcademy2.ukprn), Times.Once);
            _academiesRepository.Verify(r => r.GetAcademyByUkprn(outgoingAcademy3.ukprn), Times.Once);
            _educationPerformanceRepository.Verify(r => r.GetByAcademyUrn(outgoingAcademy1.urn), Times.Once);
            _educationPerformanceRepository.Verify(r => r.GetByAcademyUrn(outgoingAcademy2.urn), Times.Once);
            _educationPerformanceRepository.Verify(r => r.GetByAcademyUrn(outgoingAcademy3.urn), Times.Once);

            Assert.Equal(foundProjectWithMultipleAcademies, result.Project);
            Assert.Equal(3, result.OutgoingAcademies.Count);
        }
    }
}