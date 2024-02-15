using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dfe.PrepareTransfers.Extensions;

public static class DisplayExtensions
{
   public static string GetErrorStyleClass(this ModelStateDictionary modelState)
   {
      return !modelState.IsValid ? "govuk-form-group--error" : "";
   }
}
