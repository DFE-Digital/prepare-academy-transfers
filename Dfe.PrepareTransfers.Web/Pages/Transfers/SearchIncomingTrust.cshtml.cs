using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Dfe.PrepareTransfers.Web.Validators.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Helpers;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class SearchIncomingTrustModel : TransfersPageModel, ISetTrusts
    {
        private readonly ITrusts _trustsRepository;

        public SearchIncomingTrustModel(ITrusts trustsRepository)
        {
            _trustsRepository = trustsRepository;
        }

        public List<Trust> Trusts { get; private set; }

        [BindProperty(Name = "query", SupportsGet = true)]
        public string SearchQuery { get; set; } = "";
        [BindProperty]
        public string SelectedTrustId { get; set; }

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
            Trusts = result;

            var searchValidator = new IncomingTrustSearchValidator();
            var searchValidationResult = await searchValidator.ValidateAsync(this);
            if (!searchValidationResult.IsValid)
            {
                return SetErrorMessageAndRedirectToTrustName(searchValidationResult);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var validator = new IncomingTrustConfirmValidator();
            var validationResults = await validator.ValidateAsync(this);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            HttpContext.Session.SetString(IncomingTrustIdSessionKey, SelectedTrustId);

            return RedirectToAction("CheckYourAnswers", "Transfers");
        }

        private IActionResult SetErrorMessageAndRedirectToTrustName(ValidationResult validationResult)
        {
            TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
            return RedirectToPage("/Transfers/IncomingTrust", new { query = SearchQuery });
        }

        void ISetTrusts.SetTrusts(IEnumerable<Trust> trusts)
        {
           Trusts = trusts.ToList();
        }
    }
}
