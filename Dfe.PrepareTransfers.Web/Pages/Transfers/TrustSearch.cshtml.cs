using Data;
using Data.Models;
using FluentValidation.Results;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class TrustSearchModel : TransfersPageModel
    {
        public List<TrustSearchResult> Trusts;
        [BindProperty(Name = "query", SupportsGet = true)]
        public string SearchQuery { get; set; } = "";

        protected readonly ITrusts _trustsRepository;

        public TrustSearchModel(ITrusts trustsRepository)
        {
            _trustsRepository = trustsRepository;
        }

        public async Task<IActionResult> OnGetAsync(bool change = false)
        {
            ViewData["ChangeLink"] = change;

            var queryValidator = new OutgoingTrustNameValidator();
            var queryValidationResult = await queryValidator.ValidateAsync(this);
            if (!queryValidationResult.IsValid)
            {
                return SetErrorMessageAndRedirectToTrustName(queryValidationResult);
            }

            var result = await _trustsRepository.SearchTrusts(SearchQuery);
            Trusts = result.Result.Where(trust => trust.Academies.Any()).ToList();

            var searchValidator = new OutgoingTrustSearchValidator();
            var searchValidationResult = await searchValidator.ValidateAsync(this);
            if (!searchValidationResult.IsValid)
            {
                return SetErrorMessageAndRedirectToTrustName(searchValidationResult);
            }

            // We redirect here with any error messages from the subsequent
            // details step. This allows us handle errors when we visit the
            // next step via query.
            if (TempData.Peek("ErrorMessage") != null)
            {
                ModelState.AddModelError(nameof(Trusts), (string)TempData["ErrorMessage"]);
                return Page();
            }

            return Page();
        }

        private IActionResult SetErrorMessageAndRedirectToTrustName(ValidationResult validationResult)
        {
            TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
            return RedirectToPage("/Transfers/TrustName", new { query = SearchQuery });
        }
    }
}
