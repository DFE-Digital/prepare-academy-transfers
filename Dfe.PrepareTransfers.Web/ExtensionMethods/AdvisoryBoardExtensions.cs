using Dfe.Academisation.ExtensionMethods;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Extensions;

public static class AdvisoryBoardExtensions
{
   public static List<AdvisoryBoardDeferredReasonDetails> AddReasonIfValid(this List<AdvisoryBoardDeferredReasonDetails> reasons,
                                                                           bool isChecked,
                                                                           AdvisoryBoardDeferredReason reason,
                                                                           string detail,
                                                                           ModelStateDictionary modelState)
   {
      if (isChecked && string.IsNullOrWhiteSpace(detail))
      {
         modelState.AddModelError($"{reason}Details", $"Enter a reason for selecting {reason.ToDescription()}");
      }

      if (isChecked) reasons.Add(new AdvisoryBoardDeferredReasonDetails(reason, detail));

      return reasons;
   }

    public static List<AdvisoryBoardWithdrawnReasonDetails> AddReasonIfValid(this List<AdvisoryBoardWithdrawnReasonDetails> reasons,
                                                                        bool isChecked,
                                                                        AdvisoryBoardWithdrawnReason reason,
                                                                        string detail,
                                                                        ModelStateDictionary modelState)
    {
        if (isChecked && string.IsNullOrWhiteSpace(detail))
        {
            modelState.AddModelError($"{reason}Details", $"Enter a reason for selecting {reason.ToDescription()}");
        }

        if (isChecked) reasons.Add(new AdvisoryBoardWithdrawnReasonDetails(reason, detail));

        return reasons;
    }

    public static AdvisoryBoardDeferredReasonDetails GetReason(this List<AdvisoryBoardDeferredReasonDetails> reasons, AdvisoryBoardDeferredReason reason)
    {
        return reasons.FirstOrDefault(r => r.Reason == reason);
    }

    public static AdvisoryBoardWithdrawnReasonDetails GetReason(this List<AdvisoryBoardWithdrawnReasonDetails> reasons, AdvisoryBoardWithdrawnReason reason)
    {
        return reasons.FirstOrDefault(r => r.Reason == reason);
    }
}
