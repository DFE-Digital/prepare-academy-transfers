using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Frontend.Authorization
{
    public class ClaimsRequirementHandler : AuthorizationHandler<ClaimsAuthorizationRequirement>, IAuthorizationRequirement
    {
        private readonly IHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ClaimsRequirementHandler(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimsAuthorizationRequirement requirement)
        {
            if (HeaderRequirementHandler.ClientSecretHeaderValid(_environment, _httpContextAccessor, _configuration))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}