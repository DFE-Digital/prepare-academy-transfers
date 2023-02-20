using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dfe.PrepareTransfers.Web.ExtensionMethods;

public static class ModelStateDictionaryExtensions
{
   private const string FormValidationErrorClass = "govuk-form-group--error";
   private const string InputValidationErrorClass = "govuk-input--error";

   /// <summary>
   ///    Adds an "Error: " prefix to the specified title if validation has failed for the page
   /// </summary>
   /// <param name="modelState"><see cref="ModelStateDictionary" /> to check for validation failure</param>
   /// <param name="pageTitle">The title of the page</param>
   /// <returns>page title with the error prefix if the validation has failed</returns>
   public static string BuildPageTitle(this ModelStateDictionary modelState, string pageTitle)
   {
      return $"{(modelState.IsValid ? string.Empty : "Error: ")}{pageTitle}";
   }

   /// <summary>
   ///    Gets the form styling dependent on the validation state of the page
   /// </summary>
   /// <param name="modelState"><see cref="ModelStateDictionary" /> to check for validation failure</param>
   /// <returns>the CSS class to be appended to a form if the validation has failed, otherwise an empty string</returns>
   public static string GetFormGroupErrorStyle(this ModelStateDictionary modelState)
   {
      return modelState.IsValid ? string.Empty : FormValidationErrorClass;
   }

   /// <summary>
   ///    Gets the form field styling dependent on the validation state with the specified keys
   /// </summary>
   /// <param name="modelState"><see cref="ModelStateDictionary" /> to check for validation failures</param>
   /// <param name="fieldNames">set of field names for which to check validation state</param>
   /// <returns>the CSS class to be appended to an input field if the validation failed, otherwise an empty string</returns>
   /// <remarks>
   ///    <para>Will produce the error class if any of the specified fields match - an OR match</para>
   /// </remarks>
   public static string GetFormFieldErrorStyle(this ModelStateDictionary modelState, params string[] fieldNames)
   {
      var anyInvalid = fieldNames
         .Select(field => modelState.GetFieldValidationState(field) == ModelValidationState.Invalid)
         .Any(isInvalid => isInvalid);

      return anyInvalid ? InputValidationErrorClass : string.Empty;
   }
}
