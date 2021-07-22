using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.HtbDocument
{
    public class Preview : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        public string ProjectUrn => Project.Urn;
        public object OutgoingAcademyUrn => TransferringAcademy.Urn;

        public Preview(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            var response = await _getInformationForProject.Execute(id);
            Project = response.Project;
            TransferringAcademy = response.OutgoingAcademy;
            EducationPerformance = response.EducationPerformance;

            return Page();
        }
    }
}