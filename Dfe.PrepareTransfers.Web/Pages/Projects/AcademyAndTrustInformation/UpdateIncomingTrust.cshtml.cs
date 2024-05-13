using Dfe.PrepareTransfers.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Services;
using System;
using Dfe.PrepareTransfers.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Dfe.Academisation.ExtensionMethods;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation
{
    public class UpdateIncomingTrustModel : CommonPageModel
    {
        private const string SEARCH_LABEL =
           "Enter the name, UKPRN (UK Provider Reference Number) or Companies House number.";

        private const string SEARCH_ENDPOINT = "/project/0/academy-and-trust-information/update-incoming-trust?handler=Search&searchQuery=";
        private readonly ErrorService _errorService;
        private readonly ITrusts _trustsRepository;
        private readonly IProjects _repository;

        public UpdateIncomingTrustModel(ITrusts trustsRepository, IProjects projectRepository, ErrorService errorService)
        {
            _trustsRepository = trustsRepository;
            _repository = projectRepository;
            _errorService = errorService;
            AutoCompleteSearchModel = new AutoCompleteSearchModel(SEARCH_LABEL, string.Empty, SEARCH_ENDPOINT);
        }

        [BindProperty]
        public string SearchQuery { get; set; } = "";
        public AutoCompleteSearchModel AutoCompleteSearchModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int urn)
        {
            AutoCompleteSearchModel = new AutoCompleteSearchModel(SEARCH_LABEL, SearchQuery, SEARCH_ENDPOINT);

            return Page();
        }

        public async Task<IActionResult> OnGetSearch(string searchQuery)
        {
            string[] searchSplit = SplitOnBrackets(searchQuery);

            List<Trust> trusts = await _trustsRepository.SearchTrusts(searchSplit[0].Trim());

            return new JsonResult(trusts.Select(t =>
            {
                string displayUkprn = string.IsNullOrWhiteSpace(t.Ukprn) ? string.Empty : $"({t.Ukprn})";
                string suggestion = $@"{t.Name?.ToTitleCase() ?? ""} {displayUkprn}
            <br />
            Companies House number: {t.CompaniesHouseNumber ?? ""}";
                return new { suggestion = HighlightSearchMatch(suggestion, searchSplit[0].Trim(), t), value = $"{t.Name?.ToTitleCase() ?? ""} ({t.Ukprn})" };
            }));
        }

        public async Task<IActionResult> OnPostAsync(int urn)
        {
            AutoCompleteSearchModel = new AutoCompleteSearchModel(SEARCH_LABEL, SearchQuery, SEARCH_ENDPOINT);
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                ModelState.AddModelError(nameof(SearchQuery), "Enter the Trust name, UKPRN or Companies House number");
                _errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }

            string[] searchSplit = SplitOnBrackets(SearchQuery);
            if (searchSplit.Length < 2)
            {
                ModelState.AddModelError(nameof(SearchQuery), "We could not find any trusts matching your search criteria");
                _errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }

            string ukprn = searchSplit[searchSplit.Length - 1];
            string trustName = searchSplit[0];

            List<Trust> trusts = await _trustsRepository.SearchTrusts(ukprn);

            if (trusts.Count != 1)
            {
                ModelState.AddModelError(nameof(SearchQuery), "We could not find a trust matching your search criteria");
                _errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var trust = trusts.First();

                    await _repository.UpdateIncomingTrustName(urn.ToString(), trustName, ukprn);


                    return RedirectToPage("/Projects/AcademyAndTrustInformation/Index", new { urn });

                }
                catch (Exception ex)
                {

                    _errorService.AddApiError();
                    return Page();
                }

            }
            return Page();
        }

        private static string HighlightSearchMatch(string input, string toReplace, Trust trust)
        {
            if (trust == null ||
                string.IsNullOrWhiteSpace(trust.Name))
                return string.Empty;

            int index = input.IndexOf(toReplace, StringComparison.InvariantCultureIgnoreCase);
            string correctCaseSearchString = input.Substring(index, toReplace.Length);

            return input.Replace(toReplace, $"<strong>{correctCaseSearchString}</strong>", StringComparison.InvariantCultureIgnoreCase);
        }

        private static string[] SplitOnBrackets(string input)
        {
            // return array containing one empty string if input string is null or empty
            if (string.IsNullOrWhiteSpace(input)) return new string[1] { string.Empty };

            return input.Split(new[] { '(', ')' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
