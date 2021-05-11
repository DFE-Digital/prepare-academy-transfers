using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.Projects;
using Microsoft.Extensions.Logging;

namespace Data.Mock
{
    public class MockProjectRepository : IProjects
    {
        private readonly ILogger<MockProjectRepository> _logger;
        private const string PopulatedProjectUrn = "0001";
        private const string PopulatedProjectNumber = "AT-0001";
        private const string EmptyProjectUrn = "0002";
        private const string EmptyProjectNumber = "AT-0002";
        private const string OutgoingTrustName = "Example trust";

        public MockProjectRepository(ILogger<MockProjectRepository> logger)
        {
            _logger = logger;
        }

        public Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects()
        {
            var result = new RepositoryResult<List<ProjectSearchResult>>
            {
                Result = new List<ProjectSearchResult>
                {
                    new ProjectSearchResult
                    {
                        Urn = PopulatedProjectUrn, Number = PopulatedProjectNumber,
                        OutgoingTrustName = OutgoingTrustName
                    },
                    new ProjectSearchResult
                        {Urn = EmptyProjectNumber, Number = EmptyProjectNumber, OutgoingTrustName = OutgoingTrustName}
                }
            };

            return Task.FromResult(result);
        }

        public Task<RepositoryResult<Project>> GetByUrn(string urn)
        {
            return Task.FromResult(urn == PopulatedProjectUrn
                ? new RepositoryResult<Project> {Result = PopulatedProject()}
                : new RepositoryResult<Project> {Result = EmptyProject()});
        }

        public Task<RepositoryResult<Project>> Update(Project project)
        {
            _logger.LogInformation("Project {ProjectUrn} updated", project.Urn);

            return Task.FromResult(project.Urn == PopulatedProjectUrn
                ? new RepositoryResult<Project> {Result = PopulatedProject()}
                : new RepositoryResult<Project> {Result = EmptyProject()});
        }

        public Task<RepositoryResult<Project>> Create(Project project)
        {
            return Task.FromResult(new RepositoryResult<Project> {Result = EmptyProject()});
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