using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Moq;

namespace Frontend.Tests.PagesTests
{
    public abstract class PageTests
    {
        protected const string ProjectErrorUrn = "errorUrn";
        protected const string ProjectUrn = "0001";
        protected const string AcademyUrn = "1234";
        private const string AcademyName = "Academy Name";
        private const string LAName = "LA Name";
        protected readonly Mock<IGetInformationForProject> GetInformationForProject;
        protected readonly Mock<IProjects> ProjectRepository;
        protected readonly GetInformationForProjectResponse FoundInformationForProject;
        protected readonly Project FoundProjectFromRepo;

        public PageTests()
        {
            GetInformationForProject = new Mock<IGetInformationForProject>();
            FoundInformationForProject = new GetInformationForProjectResponse
            {
                Project = new Project
                {
                    Urn = ProjectUrn,
                    TransferringAcademies = new List<TransferringAcademies>
                    {
                        new TransferringAcademies()
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
                            ErrorMessage = "Error"
                        }
                    });

            ProjectRepository = new Mock<IProjects>();
            
            FoundProjectFromRepo = new Project
            {
                Urn = ProjectUrn
            };
            ProjectRepository.Setup(s => s.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = FoundProjectFromRepo
                });
        }
    }
}