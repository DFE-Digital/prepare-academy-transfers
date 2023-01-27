using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.Helpers
{
    public static class ControllerTestHelpers
    {
        public static TViewModel AssertViewModelFromResult<TViewModel>(IActionResult result)
        {
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<TViewModel>(viewResult.Model);
            return viewModel;
        }

        public static RedirectToActionResult AssertResultRedirectsToAction(IActionResult result, string actionName)
        {
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(actionName, redirectResult.ActionName);

            return redirectResult;
        }

        public static void AssertResultRedirectsToPage(IActionResult result, string expectedPageName,
            RouteValueDictionary expectedRouteValues = null)
        {
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(expectedPageName, redirectResult.PageName);
            Assert.Equal(expectedRouteValues, redirectResult.RouteValues);
        }

        public static async Task<ValidationResult> ValidateAndAddToModelState<TViewModel>(IValidator validator, TViewModel vm, ModelStateDictionary modelState)
        {
            var vc = new ValidationContext<TViewModel>(vm);
            var results = await validator.ValidateAsync(vc);
            results.AddToModelState(modelState, null);
            return results;
        }
    }
}