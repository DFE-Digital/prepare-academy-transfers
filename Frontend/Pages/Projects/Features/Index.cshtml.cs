using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.Features
{
    public class Index : CommonPageModel
    {
        private readonly IProjects _projects;
        public bool? IsSubjectToRddOrEsfaIntervention { get; set; }

        public bool HasTransferReasonBeenSet =>
            IsSubjectToRddOrEsfaIntervention != null;

        public bool IsTransferSubjectToIntervention =>
            IsSubjectToRddOrEsfaIntervention == true;

        public TransferFeatures.ProjectInitiators WhoInitiatedTheTransfer { get; set; }
        public string InterventionDetails { get; set; }
        public TransferFeatures.TransferTypes TypeOfTransfer { get; set; }
        public string OtherTypeOfTransfer { get; set; }
        [BindProperty]
        public MarkSectionCompletedViewModel MarkSectionCompletedViewModel { get; set; }

        public Index(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            
            var projectResult = project.Result;
            ProjectReference = projectResult.Reference;
            IsSubjectToRddOrEsfaIntervention =
                projectResult.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention;
            TypeOfTransfer = projectResult.Features.TypeOfTransfer;
            OtherTypeOfTransfer = projectResult.Features.OtherTypeOfTransfer;
            OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn;
            WhoInitiatedTheTransfer = projectResult.Features.WhoInitiatedTheTransfer;
            InterventionDetails = projectResult.Features.ReasonForTransfer.InterventionDetails;
            MarkSectionCompletedViewModel = new MarkSectionCompletedViewModel
            {
                IsCompleted = projectResult.Features.IsCompleted ?? false,
                ShowIsCompleted = FeaturesSectionDataIsPopulated(projectResult)
            };
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            
            var projectResult = project.Result;
            
            projectResult.Features.IsCompleted = MarkSectionCompletedViewModel.IsCompleted;

            await _projects.Update(projectResult);

            return RedirectToPage(ReturnToPreview ? Links.HeadteacherBoard.Preview.PageName : "/Projects/Index", new {Urn});
        }

        private static bool FeaturesSectionDataIsPopulated(Project project) =>
            project.Features.WhoInitiatedTheTransfer != TransferFeatures.ProjectInitiators.Empty &&
            project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention != null &&
            project.Features.TypeOfTransfer != TransferFeatures.TransferTypes.Empty;
    }
}