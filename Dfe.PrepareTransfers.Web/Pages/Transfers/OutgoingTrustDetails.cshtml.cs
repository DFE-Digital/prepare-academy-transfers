using Data;
using Data.Models;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class OutgoingTrustDetailsModel : TransfersPageModel
    {
        private readonly ITrusts _trustsRepository;

        public OutgoingTrustDetailsModel(ITrusts trustsRepository)
        {
            _trustsRepository = trustsRepository;
        }

        public Trust Trust { get; set; }
        [BindProperty(Name = "query", SupportsGet = true)]
        public string SearchQuery { get; set; } = "";
        [BindProperty(Name = "trustId", SupportsGet = true)]
        public string TrustId { get; set; }

        public async Task<IActionResult> OnGetAsync(bool change = false)
        {
            ViewData["ChangeLink"] = change;

            var validator = new OutgoingTrustConfirmValidator();
            var validationResult = await validator.ValidateAsync(this);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
                return RedirectToPage("/Transfers/TrustSearch", new { query = SearchQuery, change });
            }

            var result = await _trustsRepository.GetByUkprn(TrustId);
            Trust = result.Result;

            return Page();
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.SetString(OutgoingTrustIdSessionKey, TrustId);
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);

            return RedirectToPage("/Transfers/OutgoingTrustAcademies");
        }
    }
}
