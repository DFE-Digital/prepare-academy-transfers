using System.Threading.Tasks;
using Data.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class KeyStage2Performance : PageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        public Project Project { get; set; }
        public Academy TransferringAcademy { get; set; }
        public AdditionalInformationViewModel AdditionalInformation { get; set; }

        public KeyStage2Performance(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            if (!projectInformation.IsValid)
            {
                return RedirectToPage("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            Project = projectInformation.Project;
            TransferringAcademy = projectInformation.OutgoingAcademy;
            AdditionalInformation = new AdditionalInformationViewModel
            {
                AdditionalInformation = projectInformation.Project.KeyStage2PerformanceAdditionalInformation,
                HintText =
                    "This information will populate in your HTB template under the key stage performance tables section.",
                Urn = projectInformation.Project.Urn,
                AddOrEditAdditionalInformation = false
            };

            return Page();
        }
    }
}