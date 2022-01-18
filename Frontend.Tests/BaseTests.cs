using System;
using System.Collections.Generic;
using System.Net.Http;
using Data;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Moq;

namespace Frontend.Tests
{
    public abstract class BaseTests
    {
        protected const string ProjectUrn0001 = "0001";
        protected const string ProjectErrorUrn = "errorUrn";
        protected const string AcademyUrn = "1234";
        protected const string ErrorMessage = "Error";
        protected Mock<IGetInformationForProject> GetInformationForProject;
        protected Mock<IProjects> ProjectRepository;
        protected GetInformationForProjectResponse FoundInformationForProject;
        protected Project FoundProjectFromRepo;
        
        protected const string OutgoingAcademyName = "Academy Name";
        private const string LAName = "LA Name";

        public BaseTests()
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
                        OutgoingAcademyName = OutgoingAcademyName,
                        OutgoingAcademyUrn = AcademyUrn
                    }
                }
            };
            
            ProjectRepository.Setup(s => s.GetByUrn(It.IsAny<string>())).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = FoundProjectFromRepo
                });

            ProjectRepository.Setup(s => s.GetByUrn(ProjectErrorUrn))
                .Throws(new TramsApiException(new HttpResponseMessage(), ErrorMessage));
            
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