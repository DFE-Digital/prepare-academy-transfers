using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Errors
{
	public class IndexModel : PageModel
	{
		public string ErrorMessage { get; private set; } = "Sorry, there is a problem with the service";

		public void OnGet(int? statusCode = null)
		{
			ManageErrors(statusCode);
		}

		public void OnPost(int? statusCode = null)
		{
			ManageErrors(statusCode);
		}

		private void ManageErrors(int? statusCode)
		{
			if (!statusCode.HasValue)
			{
				ManageUnhandledErrors();
				return;
			}

			ErrorMessage = statusCode.Value switch
			{
				404 => "Page not found",
				500 => "Internal server error",
				501 => "Not implemented",
				_ => $"Error {statusCode}"
			};
		}

		private void ManageUnhandledErrors()
		{
			var unhandledError = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;

			// Thrown by RedirectToPage when the name of the page is incorrect.
			if (unhandledError is InvalidOperationException && unhandledError.Message.ToLower().Contains("no page named"))
			{
				ErrorMessage = "Page not found";
				HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
			}
		}
	}
}