using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dfe.PrepareTransfers.Web.ExtensionMethods
{
	public static class ModelStateDictionaryExtensions
	{
		public static string GetErrorStyleClass(this ModelStateDictionary modelState)
		{
			return !modelState.IsValid ? "govuk-form-group--error" : "";
		}
	}
}
