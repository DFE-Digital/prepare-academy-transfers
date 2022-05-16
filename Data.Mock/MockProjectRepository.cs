using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.Projects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Data.Mock
{
    public class MockProjectRepository : IProjects
    {
        private readonly ILogger<MockProjectRepository> _logger;
        private readonly List<Project> _projects;
        private const string PopulatedProjectUrn = "0001";
        private const string EmptyProjectUrn = "0002";

        public MockProjectRepository(ILogger<MockProjectRepository> logger)
        {
            _logger = logger;
            _projects = new List<Project> {PopulatedProject(), EmptyProject()};
        }

        public Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects(int page = 1)
        {
            var result = new RepositoryResult<List<ProjectSearchResult>>
            {
                Result = _projects.Select(project => new ProjectSearchResult
                {
                    Urn = project.Urn, OutgoingTrustName = project.OutgoingTrustName,
                    TransferringAcademies = project.TransferringAcademies
                }).ToList()
            };

            return Task.FromResult(result);
        }

        public Task<RepositoryResult<Project>> GetByUrn(string urn)
        {
            return Task.FromResult(new RepositoryResult<Project>
                {Result = CloneProject(_projects.Find(p => p.Urn == urn))});
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
            var newProject = EmptyProject();
            newProject.Urn = newProjectUrn;
            newProject.OutgoingTrustUkprn = project.OutgoingTrustUkprn;
            newProject.TransferringAcademies[0].IncomingTrustName =
                project.TransferringAcademies[0].IncomingTrustName;
            newProject.TransferringAcademies[0].IncomingTrustUkprn =
                project.TransferringAcademies[0].IncomingTrustUkprn;
            newProject.TransferringAcademies[0].OutgoingAcademyUkprn =
                project.TransferringAcademies[0].OutgoingAcademyUkprn;
            _projects.Add(newProject);
            return Task.FromResult(new RepositoryResult<Project> {Result = newProject});
        }

        private static Project CloneProject(Project toClone)
        {
            var projectString = JsonConvert.SerializeObject(toClone);
            return JsonConvert.DeserializeObject<Project>(projectString);
        }

        private static Project EmptyProject()
        {
            return new Project
            {
                Urn = EmptyProjectUrn,
                OutgoingTrustName = "The 1590 Trust",
                OutgoingTrustUkprn = "10060295",
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = "10084105",
                        OutgoingAcademyUrn = "147435",
                        OutgoingAcademyName = "Bewley Primary School",
                        IncomingTrustUkprn = "10059766",
                        IncomingTrustName = "Wise Owl Trust"
                    }
                }
            };
        }

        private static Project PopulatedProject()
        {
            return new Project
            {
                Urn = PopulatedProjectUrn,
                OutgoingTrustName = "The 1590 Trust",
                OutgoingTrustUkprn = "10060295",
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = "10040290",
                        OutgoingAcademyUrn = "139318",
                        OutgoingAcademyName = "Conyers School",
                        IncomingTrustUkprn = "10059766",
                        IncomingTrustName = "Wise Owl Trust"
                    }
                },
                Features = new TransferFeatures
                {
                    ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe,
                    TypeOfTransfer = TransferFeatures.TransferTypes.MatClosure,
                },
                Dates = new TransferDates
                {
                    Target = "01/09/2021",
                    Htb = "01/06/2021"
                },
                Benefits = new TransferBenefits
                {
                    IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                    {
                        TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                        TransferBenefits.IntendedBenefit.StrengtheningGovernance,
                        TransferBenefits.IntendedBenefit.ImprovingOfstedRating
                    },
                    OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                    {
                        {TransferBenefits.OtherFactor.HighProfile, "Some extra detail about this high profile transfer"}
                    }
                },
                Rationale = new TransferRationale
                {
                    Project = "This is the rationale for the project",
                    Trust = "This is the rationale for the trust or sponsor"
                },
                AcademyAndTrustInformation = new TransferAcademyAndTrustInformation
                {
                    Author = "Author",
                    Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve
                }
            };
        }
    }
}