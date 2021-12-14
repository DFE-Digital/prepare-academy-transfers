using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.AcademyAndTrustInformation
{
    public class Index : CommonPageModel
    {
        public string OutgoingAcademyUrn { get; set; }
        public TransferAcademyAndTrustInformation.RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
        public string ProjectName { get; set; }
        public string HtbDate { get; set; }
        public string IncomingTrustName { get; set; }
        public string TargetDate { get; set; }
        public string FirstDiscussedDate { get; set; }

        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public Index(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> OnGetAsync(string urn)
        {
            var projectInformation = await _getInformationForProject.Execute(urn);

            if (!projectInformation.IsValid)
            {
                return this.View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            OutgoingAcademyName = projectInformation.OutgoingAcademy?.Name;
            Recommendation = projectInformation.Project.AcademyAndTrustInformation.Recommendation;
            Author = projectInformation.Project.AcademyAndTrustInformation.Author;
            HtbDate = projectInformation.Project.Dates?.Htb;
            ProjectName = projectInformation.Project.Name;
            IncomingTrustName = projectInformation.Project.IncomingTrustName;
            TargetDate = projectInformation.Project.Dates?.Target;
            FirstDiscussedDate = projectInformation.Project.Dates?.FirstDiscussed;
            OutgoingAcademyUrn = projectInformation.Project.OutgoingAcademyUrn;
            Urn = projectInformation.Project.Urn;

            return Page();
        }
    }
}