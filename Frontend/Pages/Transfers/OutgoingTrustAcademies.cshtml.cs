using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.Pages.Transfers
{
    public class OutgoingTrustAcademiesModel : TransfersPageModel
    {
        public List<Academy> Academies;

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

            if (TempData.Peek("ErrorMessage") == null)
            {
                ViewData["Error.Exists"] = false;
            }
            else
            {
                ViewData["Error.Exists"] = true;
                ViewData["Error.Message"] = TempData["ErrorMessage"];
            }

            return Page();
        }
    }
}
