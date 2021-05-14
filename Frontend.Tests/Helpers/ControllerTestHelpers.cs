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
    }
}