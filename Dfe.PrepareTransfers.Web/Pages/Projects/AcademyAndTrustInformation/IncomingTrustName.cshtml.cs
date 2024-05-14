using Dfe.PrepareTransfers.Data;
using FluentValidation.AspNetCore;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation
{
    public class IncomingTrustNameModel : CommonPageModel
    {
        private readonly IProjects _projectRepository;

        public IncomingTrustNameModel(IProjects projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> OnGetAsync(string urn, bool returnToPreview = false)
        {
            var project = await _projectRepository.GetByUrn(urn);

            var projectResult = project.Result;

            Urn = projectResult.Urn;
            ReturnToPreview = returnToPreview;
            IncomingTrustName = projectResult.IncomingTrustName;
            OutgoingAcademyUrn = projectResult.OutgoingAcademyUrn;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var validator = new EditIncomingTrustNameValidator();
            var validationResults = await validator.ValidateAsync(this);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(Urn);
            }

            await _projectRepository.UpdateIncomingTrust(Urn, IncomingTrustName);

            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { Urn });
            }

            return RedirectToPage("/Projects/AcademyAndTrustInformation/Index", new { Urn });
        }
    }
}
