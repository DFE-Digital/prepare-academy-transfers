using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Pages.TaskList.Decision.Models;
using Dfe.PrepareTransfers.Services;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.PrepareTransfers.Pages.TaskList.Decision;

public class AnyConditionsModel : DecisionBaseModel
{
    private readonly ErrorService _errorService;

    public AnyConditionsModel(IProjects repository,
                              ISession session,
                           ErrorService errorService)
       : base(repository, session)
    {
        _errorService = errorService;
    }

    [BindProperty]
   [Required(ErrorMessage = "Select whether any conditions were set")]
   public bool? ApprovedConditionsSet { get; set; }

   [BindProperty]
   public string ApprovedConditionsDetails { get; set; }

   private bool HasConditions => ApprovedConditionsSet.GetValueOrDefault();

   public IActionResult OnGet(int urn)
   {
        SetBackLinkModel(Links.Decision.WhoDecided, urn);

        AdvisoryBoardDecision decision = GetDecisionFromSession(urn);
        ApprovedConditionsSet = decision.ApprovedConditionsSet;
        ApprovedConditionsDetails = decision.ApprovedConditionsDetails;

        return Page();
   }

   public IActionResult OnPost(int urn)
   {
        if (HasConditions && string.IsNullOrWhiteSpace(ApprovedConditionsDetails))
            ModelState.AddModelError(nameof(ApprovedConditionsDetails), "Add the conditions that were set");

        if (ModelState.IsValid)
        {
            AdvisoryBoardDecision decision = GetDecisionFromSession(urn);

            decision.ApprovedConditionsSet = HasConditions;
            decision.ApprovedConditionsDetails = HasConditions ? ApprovedConditionsDetails : string.Empty;

            SetDecisionInSession(urn, decision);

            return RedirectToPage(Links.Decision.DecisionDate.PageName, LinkParameters);
        }

        _errorService.AddErrors(ModelState.Keys, ModelState);
        return OnGet(urn);
   }
}
