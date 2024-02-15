using Dfe.Academisation.ExtensionMethods;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Pages.TaskList.Decision.Models;
using Dfe.PrepareTransfers.Services;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dfe.PrepareTransfers.Pages.TaskList.Decision;

public class WhoDecidedModel : DecisionBaseModel
{
    private readonly ErrorService _errorService;
    private readonly IProjects _projectsRepository;

    public WhoDecidedModel(
       IProjects repository,
       ISession session,
       ErrorService errorService
       )
      : base(repository, session)
    {
        _errorService = errorService;
    }

   [BindProperty]
   [Required(ErrorMessage = "Select who made the decision")]
   public DecisionMadeBy? DecisionMadeBy { get; set; }

   public string DecisionText { get; set; }

   public IEnumerable<DecisionMadeBy> DecisionMadeByOptions => Enum.GetValues(typeof(DecisionMadeBy)).Cast<DecisionMadeBy>();

   public IActionResult OnGet(int urn)
   {
        SetBackLinkModel(Links.Decision.RecordDecision, urn);
        AdvisoryBoardDecision decision = GetDecisionFromSession(urn);

        DecisionMadeBy = decision?.DecisionMadeBy;
        DecisionText = decision == null ? string.Empty : decision.Decision.ToDescription().ToLowerInvariant();
        return Page();
   }

   public IActionResult OnPost(int urn)
   {
        if (!ModelState.IsValid)
        {
            _errorService.AddErrors(new[] { "DecisionMadeBy" }, ModelState);
            return OnGet(urn);
        }

        AdvisoryBoardDecision decision = GetDecisionFromSession(urn) ?? new AdvisoryBoardDecision();
        decision.DecisionMadeBy = DecisionMadeBy;

        SetDecisionInSession(urn, decision);

        return decision.Decision switch
        {
            AdvisoryBoardDecisions.Approved => RedirectToPage(Links.Decision.AnyConditions.PageName, LinkParameters),
            AdvisoryBoardDecisions.Declined => RedirectToPage(Links.Decision.DeclineReason.PageName, LinkParameters),
            AdvisoryBoardDecisions.Deferred => RedirectToPage(Links.Decision.WhyDeferred.PageName, LinkParameters),
            _ => RedirectToPage(Links.Decision.AnyConditions.PageName, LinkParameters)
        };
    }
}
