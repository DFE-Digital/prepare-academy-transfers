using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Models.Features;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.HtbDocument
{
    //todo: remove models and add view models
    public class Preview : ProjectPageModel
    {
        private readonly IGetInformationForProject _getInformationForProject;
        public string ProjectUrn => Project.Urn;
        public object OutgoingAcademyUrn => TransferringAcademy.Urn;
        
        public FeaturesViewModel FeaturesViewModel { get; set; }

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

            FeaturesViewModel = new FeaturesViewModel
            {
                Urn = Project.Urn,
                IsSubjectToRddOrEsfaIntervention = Project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention,
                TypeOfTransfer = Project.Features.TypeOfTransfer,
                OtherTypeOfTransfer = Project.Features.OtherTypeOfTransfer,
                OutgoingAcademyUrn = Project.OutgoingAcademyUrn,
                WhoInitiatedTheTransfer = Project.Features.WhoInitiatedTheTransfer,
                InterventionDetails = Project.Features.ReasonForTransfer.InterventionDetails,
                ReturnToPreview = true
            };
            return Page();
        }
    }
}