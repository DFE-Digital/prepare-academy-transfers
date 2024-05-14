using Dfe.Academisation.ExtensionMethods;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Models;
using Dfe.PrepareTransfers.Pages.TaskList.Decision.Models;
using Dfe.PrepareTransfers.Services;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dfe.PrepareTransfers.Pages.TaskList.Decision;

public class DecisionDate : DecisionBaseModel, IDateValidationMessageProvider
{
    private readonly ErrorService _errorService;

    public DecisionDate(IProjects repository,
                       ISession session,
                       ErrorService errorService)
      : base(repository, session)
   {
        _errorService = errorService;
    }

   [BindProperty(Name = "decision-date", BinderType = typeof(DateInputModelBinder))]
   [DateValidation(DateRangeValidationService.DateRange.PastOrToday)]
   [Display(Name = "decision")]
   [Required]
   public DateTime? DateOfDecision { get; set; }

   public string DecisionText { get; set; }

   public AdvisoryBoardDecision Decision { get; set; }

    string IDateValidationMessageProvider.SomeMissing(string displayName, IEnumerable<string> missingParts)
    {
        return $"Date must include a {string.Join(" and ", missingParts)}";
    }

    string IDateValidationMessageProvider.AllMissing(string displayName)
    {
        string urnRaw = Request.RouteValues["urn"] as string;
        int urn = int.Parse(urnRaw ?? string.Empty);
        AdvisoryBoardDecision decision = GetDecisionFromSession(urn);
        return $"Enter the date when the conversion was {decision.Decision.ToDescription().ToLowerInvariant()}";
    }

    public LinkItem GetPageForBackLink(int urn)
    {
        return Decision switch
        {
            { Decision: AdvisoryBoardDecisions.Approved } => Links.Decision.AnyConditions,
            { Decision: AdvisoryBoardDecisions.Declined } => Links.Decision.DeclineReason,
            { Decision: AdvisoryBoardDecisions.Deferred } => Links.Decision.WhyDeferred,
            { Decision: AdvisoryBoardDecisions.Withdrawn } => Links.Decision.WhyWithdrawn,
            _ => throw new Exception("Unexpected decision state")
        };
    }

    public IActionResult OnGet(int urn)
    {
        AdvisoryBoardDecision decision = GetDecisionFromSession(urn);
        if (decision.Decision == null) return RedirectToPage(Links.Project.Index.PageName, new { urn });

        Decision = GetDecisionFromSession(urn);
        DecisionText = decision.Decision.ToString()?.ToLowerInvariant();
        DateOfDecision = Decision.AdvisoryBoardDecisionDate;

        SetBackLinkModel(GetPageForBackLink(urn), urn);

        return Page();
    }

   public IActionResult OnPost(int urn)
   {
        AdvisoryBoardDecision decision = GetDecisionFromSession(urn);
        decision.AdvisoryBoardDecisionDate = DateOfDecision;

        if (!ModelState.IsValid)
        {
            _errorService.AddErrors(Request.Form.Keys, ModelState);
            return OnGet(urn);
        }

        SetDecisionInSession(urn, decision);

        return RedirectToPage(Links.Decision.Summary.PageName, new { urn });
    }
}
