using System;
using System.Collections.Generic;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using Frontend.Controllers.Projects;
using Frontend.Models.AcademyPerformance;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class AcademyPerformanceControllerTests
    {
        private readonly AcademyPerformanceController _subject;
        private readonly Mock<IProjectsRepository> _projectRepository;
        private readonly Mock<IAcademies> _academiesRepository;
        private readonly Mock<IAcademiesRepository> _dynamicsAcademiesRepository;

        public AcademyPerformanceControllerTests()
        {
            _projectRepository = new Mock<IProjectsRepository>();
            _academiesRepository = new Mock<IAcademies>();
            _dynamicsAcademiesRepository = new Mock<IAcademiesRepository>();
            _subject = new AcademyPerformanceController(_projectRepository.Object, _academiesRepository.Object,
                _dynamicsAcademiesRepository.Object);
        }

        public class IndexTests : AcademyPerformanceControllerTests
        {
            private readonly Guid _projectId;
            private readonly GetProjectsResponseModel _foundProject;
            private readonly Academy _foundAcademy;
            private readonly GetAcademiesModel _foundDynamicsAcademy;

            public IndexTests()
            {
                _projectId = Guid.NewGuid();
                var academyId = Guid.NewGuid();

                _foundProject = new GetProjectsResponseModel
                {
                    ProjectId = _projectId,
                    ProjectAcademies = new List<GetProjectsAcademyResponseModel>
                        {new GetProjectsAcademyResponseModel {AcademyId = academyId}}
                };

                _foundDynamicsAcademy = new GetAcademiesModel
                {
                    Ukprn = "FoundUKPRN",
                    EstablishmentType = "Cat school",
                    OfstedInspectionDate = DateTime.Parse("2021-01-01")
                };

                _foundAcademy = new Academy
                {
                    Ukprn = "ukprn",
                    Performance = new AcademyPerformance()
                };


                _projectRepository.Setup(r => r.GetProjectById(_projectId)).ReturnsAsync(
                    new RepositoryResult<GetProjectsResponseModel> {Result = _foundProject});

                _dynamicsAcademiesRepository.Setup(r => r.GetAcademyById(academyId)).ReturnsAsync(
                    new RepositoryResult<GetAcademiesModel> {Result = _foundDynamicsAcademy});

                _academiesRepository.Setup(r => r.GetAcademyByUkprn("FoundUKPRN"))
                    .ReturnsAsync(new RepositoryResult<Academy> {Result = _foundAcademy});
            }

            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectId);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<AcademyPerformanceViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }

            [Fact]
            public async void GivenDynamicsAcademy_LooksUpTheAcademyWithTheUkprn()
            {
                await _subject.Index(_projectId);
                _academiesRepository.Verify(r => r.GetAcademyByUkprn("FoundUKPRN"), Times.Once);
            }

            [Fact]
            public async void GivenAcademy_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectId);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<AcademyPerformanceViewModel>(viewResponse.Model);

                Assert.Equal(_foundAcademy, viewModel.OutgoingAcademy);
            }

            [Fact]
            public async void GivenDynamicsAcademyContainsInformation_OverridesInformationFromNewAcademy()
            {
                var response = await _subject.Index(_projectId);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<AcademyPerformanceViewModel>(viewResponse.Model);
                Assert.Equal(viewModel.OutgoingAcademy.Performance.SchoolType, _foundDynamicsAcademy.EstablishmentType);
                Assert.Equal(viewModel.OutgoingAcademy.Performance.OfstedJudgementDate,
                    _foundDynamicsAcademy.OfstedInspectionDate?.ToString("d MMMM yyyy"));
            }
        }
    }
}