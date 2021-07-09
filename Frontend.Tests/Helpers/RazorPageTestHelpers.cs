using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Frontend.Tests.Helpers
{
    public static class RazorPageTestHelpers
    {
        public static TPageModel GetPageModelWithViewData<TPageModel>(params object[] parameters) where TPageModel : PageModel
        {
            var modelState = new ModelStateDictionary();
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var pageModel = (TPageModel)Activator.CreateInstance(typeof(TPageModel), parameters);
            pageModel.PageContext = pageContext;
            pageModel.TempData = tempData;
            return pageModel;
        }
    }
}