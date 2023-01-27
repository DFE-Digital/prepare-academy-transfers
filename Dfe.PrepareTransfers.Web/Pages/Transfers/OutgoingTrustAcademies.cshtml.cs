using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using FluentValidation.AspNetCore;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class OutgoingTrustAcademiesModel : TransfersPageModel
    {
        public List<Academy> Academies { get; set; }

        [BindProperty]
        public List<string> SelectedAcademyIds { get; set; }

        protected readonly ITrusts _trustsRepository;

        public OutgoingTrustAcademiesModel(ITrusts trustsRepository)
        {
            _trustsRepository = trustsRepository;
        }

        public async Task<IActionResult> OnGetAsync(bool change = false)
        {
            var sessionAcademyIds = HttpContext.Session.GetString(OutgoingAcademyIdSessionKey);
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            ViewData["OutgoingTrustId"] = outgoingTrustId;
            ViewData["ChangeLink"] = change;
            ViewData["OutgoingAcademyId"] = null;

            if (!string.IsNullOrEmpty(sessionAcademyIds))
            {
                var academyId = sessionAcademyIds.Split(",")[0];
                ViewData["OutgoingAcademyId"] = academyId;
            }

            var trustRepoResult = await _trustsRepository.GetByUkprn(outgoingTrustId);

            Academies = trustRepoResult.Result.Academies;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool change = false)
        {
            var validator = new OutgoingTrustAcademiesValidator();
            var validationResults = await validator.ValidateAsync(this);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            var academyIdsString = string.Join(",", SelectedAcademyIds);
            HttpContext.Session.SetString(OutgoingAcademyIdSessionKey, academyIdsString);

            return RedirectToAction(change ? "CheckYourAnswers" : "IncomingTrust", controllerName: "Transfers");
        }
    }
}
