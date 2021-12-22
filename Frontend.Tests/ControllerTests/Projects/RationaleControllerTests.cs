using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.Rationale;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class RationaleControllerTests
    {
        private const string _errorWithGetByUrn = "errorUrn";
        private readonly RationaleController _subject;
        private readonly Mock<IProjects> _projectRepository;
    
        public RationaleControllerTests()
        {
            _projectRepository = new Mock<IProjects>();
            _projectRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Project> {Result = new Project
                {
                    TransferringAcademies = new List<TransferringAcademies>
                    {
                        new TransferringAcademies
                        {
                            OutgoingAcademyName="Outgoing Academy"
                        }
                    }
                }});
            _projectRepository.Setup(s => s.GetByUrn(_errorWithGetByUrn))
                .ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "Error"
                        }
                    });
    
            _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                .ReturnsAsync(new RepositoryResult<Project>());
    
            _subject = new RationaleController(_projectRepository.Object);
        }
    
        public class IndexTests : RationaleControllerTests
        {
            [Fact]
            public async void GivenUrn_AssignsModelToTheView()
            {
                const string testUrn = "testUrn";
                _projectRepository.Setup(r => r.GetByUrn(testUrn))
                    .ReturnsAsync(new RepositoryResult<Project> {Result = new Project
                    {
                        Urn = testUrn,
                        TransferringAcademies = new List<TransferringAcademies>
                        {
                            new TransferringAcademies
                            {
                                OutgoingAcademyUrn= "AcademyUrn"
                            }
                        },
                        Rationale = new TransferRationale
                        {
                            Project = "projectRationale",
                            Trust = "trustRationale"
                        }
                    }});
                var result = await _subject.Index(testUrn);
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleSummaryViewModel>(result);
    
                Assert.Equal(testUrn, viewModel.Urn);
                Assert.Equal("AcademyUrn", viewModel.OutgoingAcademyUrn);
                Assert.Equal("projectRationale", viewModel.ProjectRationale);
                Assert.Equal("trustRationale", viewModel.TrustRationale);
            }
            
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.Index("0001");
    
                _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
            }
    
            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var response = await _subject.Index(_errorWithGetByUrn);
                var viewResult = Assert.IsType<ViewResult>(response);
    
                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }
    }
}