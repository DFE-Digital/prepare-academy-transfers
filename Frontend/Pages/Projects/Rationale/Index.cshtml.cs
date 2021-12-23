using System.Threading.Tasks;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.Rationale
{
    public class Index : CommonPageModel
    {
        public string ProjectRationale { get; set; }
        public string TrustRationale { get; set; }
        
        private readonly IGetInformationForProject _getInformationForProject;

        public Index(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var getInformationForProjectResponse = await _getInformationForProject.Execute(Urn);

            if (!getInformationForProjectResponse.IsValid)
            {
                return this.View("ErrorPage", getInformationForProjectResponse.ResponseError.ErrorMessage);
            }

            ProjectRationale = getInformationForProjectResponse.Project.Rationale.Project;
            TrustRationale = getInformationForProjectResponse.Project.Rationale.Trust;
            OutgoingAcademyUrn = getInformationForProjectResponse.Project.OutgoingAcademyUrn;

            return Page();
        }
    }
}