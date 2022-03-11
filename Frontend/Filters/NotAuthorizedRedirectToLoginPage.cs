using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Frontend.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NotAuthorizedRedirectToLoginPage : AllowAnonymousAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToPageResult("Login");
            }
        }
    }
}