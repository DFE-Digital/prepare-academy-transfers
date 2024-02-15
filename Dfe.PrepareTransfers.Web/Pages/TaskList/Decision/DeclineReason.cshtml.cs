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
using System.Linq;

namespace Dfe.PrepareTransfers.Pages.TaskList.Decision;

public class DeclineReasonModel : DecisionBaseModel
{
   private readonly ErrorService _errorService;

   public DeclineReasonModel(
       IProjects repository, 
       ISession session,
       ErrorService errorService)
      : base(repository, session)
   {
      _errorService = errorService;
      DeclineOptions = Enum.GetValues(typeof(AdvisoryBoardDeclinedReasons)).Cast<AdvisoryBoardDeclinedReasons>();
   }

   [BindProperty]
   public IEnumerable<string> DeclinedReasons { get; set; }

   [BindProperty]
   public string DeclineOtherReason { get; set; }

   [BindProperty]
   public string DeclineFinanceReason { get; set; }

   [BindProperty]
   public string DeclinePerformanceReason { get; set; }

   [BindProperty]
   public string DeclineGovernanceReason { get; set; }

   [BindProperty]
   public string DeclineChoiceOfTrustReason { get; set; }

   public IEnumerable<AdvisoryBoardDeclinedReasons> DeclineOptions { get; }
   public string DecisionText { get; set; }

   public UIHelpers UI => new(this);

   public IActionResult OnGet(int urn)
   {
        PreloadStateFromSession(urn);
        SetBackLinkModel(Links.Decision.WhoDecided, urn);

        return Page();
   }

   public IActionResult OnPost(int urn)
   {
      AdvisoryBoardDecision decision = GetDecisionFromSession(urn);

      if (DeclinedReasons.Any())
      {
         decision.DeclinedReasons.Clear();
         decision.DeclinedReasons.AddRange(MapSelectedReasons());
      }
      else
      {
         ModelState.AddModelError("DeclinedReasonSet", "Select at least one reason");
      }

      EnsureExplanationIsProvidedFor(AdvisoryBoardDeclinedReasons.Finance, DeclineFinanceReason);
      EnsureExplanationIsProvidedFor(AdvisoryBoardDeclinedReasons.Performance, DeclinePerformanceReason);
      EnsureExplanationIsProvidedFor(AdvisoryBoardDeclinedReasons.Governance, DeclineGovernanceReason);
      EnsureExplanationIsProvidedFor(AdvisoryBoardDeclinedReasons.ChoiceOfTrust, DeclineChoiceOfTrustReason);
      EnsureExplanationIsProvidedFor(AdvisoryBoardDeclinedReasons.Other, DeclineOtherReason);

      SetDecisionInSession(urn, decision);

        if (ModelState.IsValid) return RedirectToPage(Links.Decision.DecisionDate.PageName, LinkParameters);

        _errorService.AddErrors(ModelState.Keys, ModelState);
        return OnGet(urn);
   }

   private IEnumerable<AdvisoryBoardDeclinedReasonDetails> MapSelectedReasons()
   {
      return DeclinedReasons
         .Select(reasonText => Enum.Parse<AdvisoryBoardDeclinedReasons>(reasonText, true))
         .Select(reason => reason switch
         {
            AdvisoryBoardDeclinedReasons.Finance => new AdvisoryBoardDeclinedReasonDetails(reason, DeclineFinanceReason),
            AdvisoryBoardDeclinedReasons.Performance => new AdvisoryBoardDeclinedReasonDetails(reason, DeclinePerformanceReason),
            AdvisoryBoardDeclinedReasons.Governance => new AdvisoryBoardDeclinedReasonDetails(reason, DeclineGovernanceReason),
            AdvisoryBoardDeclinedReasons.ChoiceOfTrust => new AdvisoryBoardDeclinedReasonDetails(reason, DeclineChoiceOfTrustReason),
            AdvisoryBoardDeclinedReasons.Other => new AdvisoryBoardDeclinedReasonDetails(reason, DeclineOtherReason),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unexpected value for AdvisoryBoardDeclinedReason")
         });
   }

   private void PreloadStateFromSession(int urn)
   {
      AdvisoryBoardDecision decision = GetDecisionFromSession(urn);
      DecisionText = decision.Decision.ToDescription().ToLowerInvariant();

      Dictionary<AdvisoryBoardDeclinedReasons, string> reasons = decision.DeclinedReasons?.ToDictionary(key => key.Reason, value => value.Details);

      if (reasons == null) return;

      DeclinedReasons = reasons.Select(r => r.Key.ToString());
      DeclineOtherReason = reasons.GetValueOrDefault(AdvisoryBoardDeclinedReasons.Other);
      DeclineFinanceReason = reasons.GetValueOrDefault(AdvisoryBoardDeclinedReasons.Finance);
      DeclinePerformanceReason = reasons.GetValueOrDefault(AdvisoryBoardDeclinedReasons.Performance);
      DeclineGovernanceReason = reasons.GetValueOrDefault(AdvisoryBoardDeclinedReasons.Governance);
      DeclineChoiceOfTrustReason = reasons.GetValueOrDefault(AdvisoryBoardDeclinedReasons.ChoiceOfTrust);
   }

   private void EnsureExplanationIsProvidedFor(AdvisoryBoardDeclinedReasons reason, string explanation)
   {
      string reasonName = reason.ToString();

      if (DeclinedReasons.Contains(reasonName) && string.IsNullOrWhiteSpace(explanation))
         ModelState.AddModelError(UI.ReasonFieldFor(reason), $"Enter a reason for selecting {reason.ToDescription()}");
   }

   public class UIHelpers
   {
      private readonly DeclineReasonModel _model;

      public UIHelpers(DeclineReasonModel model)
      {
         _model = model;
      }

      public bool IsChecked(AdvisoryBoardDeclinedReasons reason)
      {
         return _model.DeclinedReasons.Contains(reason.ToString());
      }

      public string IdFor(string prefix, object item)
      {
         string connector = prefix.EndsWith('-') ? string.Empty : "-";
         string suffix = item?.ToString()?.ToLowerInvariant();

         return $"{prefix}{connector}{suffix}";
      }

      public string ValueFor(object item)
      {
         return item.ToString();
      }

      public string ReasonFieldFor(object item)
      {
         return $"Decline{item}Reason";
      }

      public string ReasonValueFor(AdvisoryBoardDeclinedReasons reason)
      {
         return reason switch
         {
            AdvisoryBoardDeclinedReasons.Finance => _model.DeclineFinanceReason,
            AdvisoryBoardDeclinedReasons.Performance => _model.DeclinePerformanceReason,
            AdvisoryBoardDeclinedReasons.Governance => _model.DeclineGovernanceReason,
            AdvisoryBoardDeclinedReasons.ChoiceOfTrust => _model.DeclineChoiceOfTrustReason,
            AdvisoryBoardDeclinedReasons.Other => _model.DeclineOtherReason,
            _ => string.Empty
         };
      }
   }
}
