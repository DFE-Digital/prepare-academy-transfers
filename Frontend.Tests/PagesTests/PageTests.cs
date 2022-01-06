using System.Collections.Generic;
using System.Net;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Moq;

namespace Frontend.Tests.PagesTests
{
    public abstract class PageTests
    {
        protected const string ProjectErrorUrn = "errorUrn";
        protected const string ProjectUrn0001 = "0001";
        protected const string AcademyUrn = "1234";
        protected const string ErrorMessage = "Error";
        protected const string ErrorPageName = "ErrorPage";
        protected Mock<IGetInformationForProject> GetInformationForProject;
        protected Mock<IProjects> ProjectRepository;
        protected GetInformationForProjectResponse FoundInformationForProject;
        protected Project FoundProjectFromRepo;
        
        private const string AcademyName = "Academy Name";
        private const string LAName = "LA Name";

        public PageTests()
        {
            MockGetInformationForProject();
            MockProjectRepository();
        }

        private void MockProjectRepository()
        {
            ProjectRepository = new Mock<IProjects>();

            FoundProjectFromRepo = new Project
            {
                Urn = ProjectUrn0001,
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies()
                    {
                        OutgoingAcademyName = AcademyName
                    }
                }
            };
            
            ProjectRepository.Setup(s => s.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = FoundProjectFromRepo
                });
            
            ProjectRepository.Setup(s => s.GetByUrn(ProjectErrorUrn)).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Error = new RepositoryResultBase.RepositoryError()
                    {
                        ErrorMessage = ErrorMessage,
                        StatusCode = HttpStatusCode.UnavailableForLegalReasons
                    }
                });
            
            ProjectRepository.Setup(r => r.Update(It.IsAny<Project>()))
            .ReturnsAsync(new RepositoryResult<Project>());
        }

        private void MockGetInformationForProject()
        {
            GetInformationForProject = new Mock<IGetInformationForProject>();
            FoundInformationForProject = new GetInformationForProjectResponse
            {
                Project = new Project
                {
                    Urn = ProjectUrn0001,
                    TransferringAcademies = new List<TransferringAcademies>
                    {
                        new TransferringAcademies
                        {
                            OutgoingAcademyUrn = AcademyUrn
                        }
                    }
                },
                OutgoingAcademy = new Academy
                {
                    Urn = AcademyUrn,
                    LocalAuthorityName = LAName,
                    Name = AcademyName
                }
            };

            GetInformationForProject.Setup(s => s.Execute(It.IsAny<string>()))
                .ReturnsAsync(
                    FoundInformationForProject
                );

            GetInformationForProject.Setup(s => s.Execute(ProjectErrorUrn))
                .ReturnsAsync(
                    new GetInformationForProjectResponse
                    {
                        ResponseError = new ServiceResponseError
                        {
                            ErrorMessage = ErrorMessage
                        }
                    });
        }
    }
}