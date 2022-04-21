using Data;
using Data.Models;
using Frontend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Pages.Transfers
{
    public class CheckYourAnswersModel : TransfersPageModel
    {
        private readonly ITrusts _trustsRepository;

        public CheckYourAnswersModel(ITrusts trustsRepository)
        {
            _trustsRepository = trustsRepository;
        }
        
        [BindProperty]
        public Trust OutgoingTrust { get; set; }
        [BindProperty]
        public List<Academy> OutgoingAcademies { get; set; }
        [BindProperty]
        public Trust IncomingTrust { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            IncomingTrust = null;
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey);

            var outgoingTrustResponse = await _trustsRepository.GetByUkprn(outgoingTrustId);
            OutgoingTrust = outgoingTrustResponse.Result;

            var incomingTrustIdString = HttpContext.Session.GetString(IncomingTrustIdSessionKey);

            if (incomingTrustIdString != null)
            {
                var incomingTrustResponse = await _trustsRepository.GetByUkprn(incomingTrustIdString);

                IncomingTrust = incomingTrustResponse.Result;
            }

            OutgoingAcademies = outgoingTrustResponse.Result.Academies
                .Where(academy => academyIds.Contains(academy.Ukprn)).ToList();

            return Page();
        }
    }
}
