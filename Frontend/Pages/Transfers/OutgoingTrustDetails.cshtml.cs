using Data;
using Data.Models;
using Frontend.Validators.Transfers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Pages.Transfers
{
    public class OutgoingTrustDetailsModel : PageModel
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
    }
}
