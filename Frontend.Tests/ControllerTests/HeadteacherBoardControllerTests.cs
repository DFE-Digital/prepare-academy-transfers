using System;
using System.Collections.Generic;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Frontend.Controllers;
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

        public HeadteacherBoardControllerTests()
        {
            _projectsRepository = new Mock<IProjectsRepository>();
            _createHtbDocument = new Mock<ICreateHtbDocument>();

            _subject = new HeadteacherBoardController(_projectsRepository.Object, _createHtbDocument.Object);
        }

        public class PreviewTests : HeadteacherBoardControllerTests
        {
            private readonly Guid _projectId;

            public PreviewTests()
            {
                _projectId = Guid.NewGuid();

                _projectsRepository.Setup(r => r.GetProjectById(_projectId)).ReturnsAsync(
                    new RepositoryResult<GetProjectsResponseModel>
                    {
                        Result = new GetProjectsResponseModel
                        {
                            ProjectId = _projectId,
                            ProjectAcademies = new List<GetProjectsAcademyResponseModel>
                            {
                                new GetProjectsAcademyResponseModel {AcademyName = "Cat Academy"}
                            }
                        }
                    });
            }

            [Fact]
            public async void GivenId_GetsTheProjectFromTheProjectRepository()
            {
                await _subject.Preview(_projectId);
                _projectsRepository.Verify(r => r.GetProjectById(_projectId), Times.Once);
            }

            [Fact]
            public async void GivenId_PutsTheProjectIdAndAcademyNameInTheViewData()
            {
                await _subject.Preview(_projectId);

                Assert.Equal(_projectId, _subject.ViewData["ProjectId"]);
                Assert.Equal("Cat Academy", _subject.ViewData["AcademyName"]);
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