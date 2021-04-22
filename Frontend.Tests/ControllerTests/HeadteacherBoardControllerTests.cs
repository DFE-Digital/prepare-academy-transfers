using System;
using System.Collections.Generic;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using Frontend.Controllers;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class HeadteacherBoardControllerTests
    {
        private readonly HeadteacherBoardController _subject;
        private readonly Mock<IProjectsRepository> _projectsRepository;
        private readonly Mock<ICreateHtbDocument> _createHtbDocument;
        private readonly Mock<IAcademiesRepository> _dynamicsAcademiesRepository;
        private readonly Mock<IAcademies> _academiesRepository;

        public HeadteacherBoardControllerTests()
        {
            _projectsRepository = new Mock<IProjectsRepository>();
            _createHtbDocument = new Mock<ICreateHtbDocument>();
            _dynamicsAcademiesRepository = new Mock<IAcademiesRepository>();
            _academiesRepository = new Mock<IAcademies>();

            _subject = new HeadteacherBoardController(_projectsRepository.Object, _createHtbDocument.Object,
                _dynamicsAcademiesRepository.Object, _academiesRepository.Object);
        }

        public class PreviewTests : HeadteacherBoardControllerTests
        {
            private readonly Guid _projectId;
            private readonly Guid _academyId;

            public PreviewTests()
            {
                _projectId = Guid.NewGuid();
                _academyId = Guid.NewGuid();

                _projectsRepository.Setup(r => r.GetProjectById(_projectId)).ReturnsAsync(
                    new RepositoryResult<GetProjectsResponseModel>
                    {
                        Result = new GetProjectsResponseModel
                        {
                            ProjectId = _projectId,
                            ProjectAcademies = new List<GetProjectsAcademyResponseModel>
                            {
                                new GetProjectsAcademyResponseModel
                                    {AcademyId = _academyId, AcademyName = "Cat Academy"}
                            }
                        }
                    });

                _dynamicsAcademiesRepository.Setup(r => r.GetAcademyById(_academyId)).ReturnsAsync(
                    new RepositoryResult<GetAcademiesModel>
                        {Result = new GetAcademiesModel {Id = _academyId, Ukprn = "FoundUkprn"}}
                );

                _academiesRepository.Setup(r => r.GetAcademyByUkprn("FoundUkprn")).ReturnsAsync(
                    new RepositoryResult<Academy> {Result = new Academy {Ukprn = "FoundNonDynamicsUkprn"}});
            }

            [Fact]
            public async void GivenId_GetsTheProjectFromTheProjectRepository()
            {
                await _subject.Preview(_projectId);
                _projectsRepository.Verify(r => r.GetProjectById(_projectId), Times.Once);
            }

            [Fact]
            public async void GivenId_PutsTheProjectInTheViewModel()
            {
                var response = await _subject.Preview(_projectId);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<HeadTeacherBoardPreviewViewModel>(viewResponse.Model);

                Assert.Equal(_projectId, viewModel.Project.ProjectId);
            }

            [Fact]
            public async void GivenId_PutsTheOutgoingAcademyInTheViewModel()
            {
                var response = await _subject.Preview(_projectId);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<HeadTeacherBoardPreviewViewModel>(viewResponse.Model);

                Assert.Equal("FoundNonDynamicsUkprn", viewModel.OutgoingAcademy.Ukprn);
            }
        }

        public class GenerateDocumentTests : HeadteacherBoardControllerTests
        {
            [Fact]
            public async void GivenId_GeneratesAnHtbDocumentForTheProject()
            {
                var projectId = Guid.NewGuid();
                await _subject.GenerateDocument(projectId);

                _createHtbDocument.Verify(s => s.Execute(projectId), Times.Once);
            }

            [Fact]
            public async void GivenId_ReturnsAFileWithTheGeneratedDocument()
            {
                var projectId = Guid.NewGuid();
                var fileContents = new byte[] {1, 2, 3, 4};
                _createHtbDocument.Setup(s => s.Execute(projectId)).ReturnsAsync(fileContents);
                var response = await _subject.GenerateDocument(projectId);
                var fileResponse = Assert.IsType<FileContentResult>(response);

                Assert.Equal(fileContents, fileResponse.FileContents);
            }
        }
    }
}