using Data;
using Data.Models;
using FluentValidation.Results;
using Frontend.Validators.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Pages.Transfers
{
    public class SearchIncomingTrustModel : TransfersPageModel
    {
        private readonly ITrusts _trustsRepository;

        public SearchIncomingTrustModel(ITrusts trustsRepository)
        {
            _trustsRepository = trustsRepository;
        }

        public List<TrustSearchResult> Trusts;
        [BindProperty(Name = "query", SupportsGet = true)]
        public string SearchQuery { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(bool change = false)
        {
            ViewData["ChangeLink"] = change;

            var queryValidator = new IncomingTrustNameValidator();
            var queryValidationResult = await queryValidator.ValidateAsync(this);
            if (!queryValidationResult.IsValid)
            {
                return SetErrorMessageAndRedirectToTrustName(queryValidationResult);
            }

            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            var result = await _trustsRepository.SearchTrusts(SearchQuery, outgoingTrustId);
            Trusts = result.Result;

            var searchValidator = new IncomingTrustSearchValidator();
            var searchValidationResult = await searchValidator.ValidateAsync(this);
            if (!searchValidationResult.IsValid)
            {
                return SetErrorMessageAndRedirectToTrustName(searchValidationResult);
            }

            // We redirect here with any error messages from the subsequent
            // steps. This allows us handle errors when we visit the
            // next step via query.
            if (TempData.Peek("ErrorMessage") != null)
            {
                ModelState.AddModelError(nameof(Trusts), (string)TempData["ErrorMessage"]);
            }

            return Page();
        }

        private IActionResult SetErrorMessageAndRedirectToTrustName(ValidationResult validationResult)
        {
            TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
            return RedirectToPage("/Transfers/IncomingTrust", new { query = SearchQuery });
        }
    }
}
