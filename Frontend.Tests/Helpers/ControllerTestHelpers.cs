using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Frontend.Tests.Helpers
{
    public static class ControllerTestHelpers
    {
        public static TViewModel GetViewModelFromResult<TViewModel>(IActionResult result)
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
    }
}