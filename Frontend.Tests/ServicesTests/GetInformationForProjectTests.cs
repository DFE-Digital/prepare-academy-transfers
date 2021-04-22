using System;
using System.Collections.Generic;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using Frontend.Services;
using Moq;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class GetInformationForProjectTests
    {
        private readonly GetInformationForProject _subject;
        private readonly Mock<IProjectsRepository> _projectsRepository;
        private readonly Mock<IAcademiesRepository> _dynamicsAcademiesRepository;
        private readonly Mock<IAcademies> _academiesRepository;
        private readonly Guid _projectId;
        private readonly Guid _academyDynamicsId;
        private GetProjectsResponseModel _foundProject;
        private Academy _foundAcademy;

        public GetInformationForProjectTests()
        {
            _projectId = Guid.NewGuid();
            _academyDynamicsId = Guid.NewGuid();
            _projectsRepository = new Mock<IProjectsRepository>();
            _dynamicsAcademiesRepository = new Mock<IAcademiesRepository>();
            _academiesRepository = new Mock<IAcademies>();

            _subject = new GetInformationForProject(_projectsRepository.Object, _dynamicsAcademiesRepository.Object,
                _academiesRepository.Object);

            SetupRepositories();
        }

        private void SetupRepositories()
        {
            _foundProject = new GetProjectsResponseModel
            {
                ProjectId = _projectId,
                ProjectAcademies = new List<GetProjectsAcademyResponseModel>
                {
                    new GetProjectsAcademyResponseModel {AcademyId = _academyDynamicsId}
                }
            };

            _foundAcademy = new Academy()
            {
                Ukprn = "AcademyUkprn"
            };

            _projectsRepository.Setup(r => r.GetProjectById(_projectId)).ReturnsAsync(
                new RepositoryResult<GetProjectsResponseModel>()
                {
                    Result = _foundProject
                });

            _dynamicsAcademiesRepository.Setup(r => r.GetAcademyById(_academyDynamicsId)).ReturnsAsync(
                new RepositoryResult<GetAcademiesModel>
                {
                    Result = new GetAcademiesModel
                    {
                        Ukprn = "DynamicsAcademyUkprn"
                    }
                });

            _academiesRepository.Setup(r => r.GetAcademyByUkprn("DynamicsAcademyUkprn")).ReturnsAsync(
                new RepositoryResult<Academy>
                {
                    Result = _foundAcademy
                });
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectInRepository()
        {
            await _subject.Execute(_projectId);

            _projectsRepository.Verify(r => r.GetProjectById(_projectId), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_LooksUpProjectAcademyInDynamics()
        {
            await _subject.Execute(_projectId);

            _dynamicsAcademiesRepository.Verify(r => r.GetAcademyById(_academyDynamicsId), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_LooksUpAcademyByDynamicsAcademyUkprn()
        {
            await _subject.Execute(_projectId);

            _academiesRepository.Verify(r => r.GetAcademyByUkprn("DynamicsAcademyUkprn"), Times.Once);
        }

        [Fact]
        public async void GivenProjectId_ReturnsFoundProjectAndAcademy()
        {
            var result = await _subject.Execute(_projectId);

            Assert.Equal(result.Project, _foundProject);
            Assert.Equal(result.OutgoingAcademy, _foundAcademy);
        }
    }
}