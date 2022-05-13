using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace Frontend.Validation
{
    //Handler is registered from the method RequireAuthenticatedUser()
    public class HeaderRequirementHandler : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>, IAuthorizationRequirement
    {
        private readonly IHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public HeaderRequirementHandler(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyAnonymousAuthorizationRequirement requirement)
        {
            //Header authorisation not applicable for production
            if (_environment.IsStaging() || _environment.IsDevelopment())
            {
                //Allow client secret in header
                var authHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString()?
                    .Replace("Bearer ", String.Empty);
                if (authHeader == _configuration.GetSection("AzureAd")["ClientSecret"])
                {
                    context.Succeed(requirement);  
                } 
            }
            return Task.CompletedTask;
        }
    }
}