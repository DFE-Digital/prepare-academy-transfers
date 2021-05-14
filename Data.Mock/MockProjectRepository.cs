using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.Projects;
using Microsoft.Extensions.Logging;

namespace Data.Mock
{
    public class MockProjectRepository : IProjects
    {
        private readonly ILogger<MockProjectRepository> _logger;
        private readonly List<Project> _projects;
        private const string PopulatedProjectUrn = "0001";
        private const string PopulatedProjectNumber = "AT-0001";
        private const string EmptyProjectUrn = "0002";
        private const string EmptyProjectNumber = "AT-0002";
        private const string OutgoingTrustName = "Example trust";

        public MockProjectRepository(ILogger<MockProjectRepository> logger)
        {
            _logger = logger;
            _projects = new List<Project> {PopulatedProject(), EmptyProject()};
        }

        public Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects()
        {
            var result = new RepositoryResult<List<ProjectSearchResult>>
            {
                Result = _projects.Select(project => new ProjectSearchResult
                    {Urn = project.Urn, Number = project.Name, OutgoingTrustName = project.OutgoingTrustName}).ToList()
            };

            return Task.FromResult(result);
        }

        public Task<RepositoryResult<Project>> GetByUrn(string urn)
        {
            return Task.FromResult(new RepositoryResult<Project> {Result = _projects.Find(p => p.Urn == urn)});
        }

        public Task<RepositoryResult<Project>> Update(Project project)
        {
            var projectIndex = _projects.FindIndex(p => p.Urn == project.Urn);
            _projects[projectIndex] = project;

            _logger.LogInformation("Project {ProjectUrn} updated", project.Urn);

            return Task.FromResult(new RepositoryResult<Project> {Result = project});
        }

        public Task<RepositoryResult<Project>> Create(Project project)
        {
            var newProjectNumber = _projects.Max(p => int.Parse(p.Urn)) + 1;
            var newProjectUrn = newProjectNumber.ToString().PadLeft(4, '0');
            var newProjectName = $"AT-{newProjectUrn}";
            var newProject = EmptyProject();
            newProject.Urn = newProjectUrn;
            newProject.Name = newProjectName;
            newProject.OutgoingTrustUkprn = project.OutgoingTrustUkprn;
            newProject.TransferringAcademies[0].IncomingTrustUkprn =
                project.TransferringAcademies[0].IncomingTrustUkprn;
            newProject.TransferringAcademies[0].OutgoingAcademyUkprn =
                project.TransferringAcademies[0].OutgoingAcademyUkprn;
            _projects.Add(newProject);
            return Task.FromResult(new RepositoryResult<Project> {Result = newProject});
        }

        private static Project EmptyProject()
        {
            return new Project
            {
                Urn = EmptyProjectUrn,
                Name = EmptyProjectNumber,
                OutgoingTrustName = OutgoingTrustName,
                OutgoingTrustUkprn = "1111111",
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = "3333333",
                        IncomingTrustUkprn = "2222222"
                    }
                },
                Features = new TransferFeatures
                {
                    ReasonForTransfer = new ReasonForTransfer()
                }
            };
        }

        private static Project PopulatedProject()
        {
            return new Project
            {
                Urn = PopulatedProjectUrn,
                Name = PopulatedProjectNumber,
                OutgoingTrustName = OutgoingTrustName,
                OutgoingTrustUkprn = "1111111",
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = "3333333",
                        IncomingTrustUkprn = "2222222"
                    }
                },
                Features = new TransferFeatures
                {
                    WhoInitiatedTheTransfer = TransferFeatures.ProjectInitiators.Dfe,
                    ReasonForTransfer = new ReasonForTransfer
                    {
                        IsSubjectToRddOrEsfaIntervention = true,
                        InterventionDetails = "Intervention details"
                    }
                }
            };
        }
    }
}