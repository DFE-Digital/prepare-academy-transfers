using System.Collections.Generic;
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
        protected const string ProjectUrn0001 = "0001";
        protected const string AcademyUrn = "1234";
        protected Mock<IGetInformationForProject> GetInformationForProject;
        protected Mock<IProjects> ProjectRepository;
        protected GetInformationForProjectResponse FoundInformationForProject;
        protected Project FoundProjectFromRepo;
        
        private const string OutgoingAcademyName = "Academy Name";
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
                        OutgoingAcademyName = OutgoingAcademyName
                    }
                }
            };
            
            ProjectRepository.Setup(s => s.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = FoundProjectFromRepo
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
                    Name = OutgoingAcademyName
                }
            };

            GetInformationForProject.Setup(s => s.Execute(It.IsAny<string>()))
                .ReturnsAsync(
                    FoundInformationForProject
                );
        }
    }
}