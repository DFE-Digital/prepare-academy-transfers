using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Frontend.ExtensionMethods
{
	public static class ModelStateDictionaryExtensions
	{
		public static string GetErrorStyleClass(this ModelStateDictionary modelState)
		{
			return !modelState.IsValid ? "govuk-form-group--error" : "";
		}
	}
}
