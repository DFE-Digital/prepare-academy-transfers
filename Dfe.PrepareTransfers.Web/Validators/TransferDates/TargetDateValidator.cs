using Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using FluentValidation;

namespace Dfe.PrepareTransfers.Web.Validators.TransferDates;

public class TargetDateValidator : AbstractValidator<TargetDateViewModel>
{
   public TargetDateValidator()
   {
      ClassLevelCascadeMode = CascadeMode.Stop;

      RuleFor(x => x.TargetDate)
         .SetValidator(new DateValidator { ErrorDisplayName = "expected transfer date" });

      RuleFor(x => x.TargetDate)
         .SetValidator(new FutureDateValidator());

      RuleFor(x => x.TargetDate.Date.Day)
         .Custom((day, context) =>
         {
            if (!context.RootContextData.TryGetValue("AdvisoryBoardDate", out var advisoryBoardDate)) return;
            if (string.IsNullOrWhiteSpace((string)advisoryBoardDate)) return;

            TargetDateViewModel dateVm = context.InstanceToValidate;
            if (!dateVm.TargetDate.UnknownDate && DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(
                   (string)advisoryBoardDate, dateVm.TargetDate.DateInputAsString()))
               context.AddFailure(
                  "The target transfer date must be on or after the Advisory board date");
         });
   }
}
