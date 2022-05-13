using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace Frontend.Authorization
{
    //Handler is registered from the method RequireAuthenticatedUser()
    public class HeaderRequirementHandler : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>,
        IAuthorizationRequirement
    {
        private readonly IHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public HeaderRequirementHandler(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        /// <summary>
        /// Checks for a value in Authorization header of the request
        /// If this matches the Secret, Authorization is granted on Dev/Staging
        /// </summary>
        /// <param name="hostEnvironment">Environment</param>
        /// <param name="httpContextAccessor">Used to check header bearer token value </param>
        /// <param name="configuration">Used to access secret value</param>
        /// <returns>True if secret and header value match</returns>
        public static bool ClientSecretHeaderValid(IHostEnvironment hostEnvironment,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            //Header authorisation not applicable for production
            string authHeader = null;
            if (hostEnvironment.IsStaging() || hostEnvironment.IsDevelopment())
            {
                //Allow client secret in header
                authHeader = httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString()?
                    .Replace("Bearer ", string.Empty);
            }

            return authHeader == configuration.GetSection("AzureAd")["ClientSecret"];
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            DenyAnonymousAuthorizationRequirement requirement)
        {
            if (ClientSecretHeaderValid(_environment, _httpContextAccessor, _configuration))
            {
                context.Succeed(requirement);
                //todo: move this to header param
                context.User.Identities.FirstOrDefault()?.AddClaim(new Claim(ClaimTypes.Role, "transfers.create"));
            }
            return Task.CompletedTask;
        }
    }
}