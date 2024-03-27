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
    public class ProposedTrustNameModel : TransfersPageModel
    {
        public ProposedTrustNameModel()
        {
        }

        [BindProperty]
        public string ProposedTrustName { get; set; }

        public async Task<IActionResult> OnGetAsync(bool change = false)
        {
            ViewData["ChangeLink"] = change;



            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var validator = new ProposedTrustNameValidator();
            var validationResults = await validator.ValidateAsync(this);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            HttpContext.Session.SetString(ProposedTrustNameSessionKey, ProposedTrustName);

            return RedirectToAction("CheckYourAnswers", "Transfers");
        }
    }
}
