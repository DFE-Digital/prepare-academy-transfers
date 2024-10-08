using Dfe.Academisation.CorrelationIdMiddleware;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Data.TRAMS;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Request;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models.EducationPerformance;
using Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Authorization;
using Dfe.PrepareTransfers.Web.BackgroundServices;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Options;
using Dfe.PrepareTransfers.Web.Security;
using Dfe.PrepareTransfers.Web.Services;
using Dfe.PrepareTransfers.Web.Services.AzureAd;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Dfe.PrepareTransfers.Web.Validators.Features;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dfe.Academies.Contracts.V4.Trusts;
using Dfe.Academies.Contracts.V4.Establishments;
using Dfe.PrepareTransfers.Services;
using Dfe.PrepareTransfers.Data.Services.Interfaces;
using Dfe.PrepareTransfers.Data.Services;
using Dfe.PrepareTransfers.Web.Routing;

namespace Dfe.PrepareTransfers.Web;

public class Startup
{
    private readonly TimeSpan _authenticationExpiration;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        _authenticationExpiration =
           TimeSpan.FromMinutes(int.Parse(Configuration["AuthenticationExpirationInMinutes"] ?? "60"));
    }

    private IConfiguration Configuration { get; }

    private IConfigurationSection GetConfigurationSection<T>()
    {
        string sectionName = typeof(T).Name.Replace("Options", string.Empty);
        return Configuration.GetRequiredSection(sectionName);
    }

    private T GetTypedConfiguration<T>()
    {
        return GetConfigurationSection<T>().Get<T>();
    }


    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
           .AddRazorPages(options =>
           {
               options.Conventions.AuthorizeFolder("/");
               options.Conventions.AllowAnonymousToPage("/AccessibilityStatement");
               options.Conventions.AllowAnonymousToPage("/Maintenance");
               options.Conventions.AllowAnonymousToPage("/SessionTimedOut");
           })
           .AddViewOptions(options => { options.HtmlHelperOptions.ClientValidationEnabled = false; }).AddMvcOptions(options =>
           {
               options.MaxModelValidationErrors = 50;
               options.Filters.Add(new MaintenancePageFilter(Configuration));
           });

        services.AddControllersWithViews(options => options.Filters.Add(
              new AutoValidateAntiforgeryTokenAttribute()))
           .AddSessionStateTempDataProvider()
           .AddMicrosoftIdentityUI();


        services.Configure<ServiceLinkOptions>(GetConfigurationSection<ServiceLinkOptions>());

        services.AddApplicationInsightsTelemetry();
        services
           .AddFluentValidationAutoValidation()
           .AddFluentValidationClientsideAdapters()
           .AddValidatorsFromAssemblyContaining<FeaturesReasonValidator>();

        services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        services.Configure<AzureAdOptions>(GetConfigurationSection<AzureAdOptions>());

        services.AddHealthChecks();
        AddServices(services, Configuration);

        AuthorizationPolicyBuilder policyBuilder = SetupAuthorizationPolicyBuilder();
        services.AddAuthorization(options => { options.DefaultPolicy = policyBuilder.Build(); });

        services.AddScoped(sp => sp.GetService<IHttpContextAccessor>()?.HttpContext?.Session);
        services.AddSession(options =>
        {
            options.IdleTimeout = _authenticationExpiration;
            options.Cookie.Name = ".ManageAnAcademyTransfer.Session";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;

            if (string.IsNullOrEmpty(Configuration["CI"]))
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            }
        });
        services.AddMicrosoftIdentityWebAppAuthentication(Configuration);
        services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
           options =>
           {
               options.AccessDeniedPath = "/access-denied";
               options.Cookie.Name = ".ManageAnAcademyTransfer.Login";
               options.Cookie.HttpOnly = true;
               options.Cookie.IsEssential = true;
               options.ExpireTimeSpan = _authenticationExpiration;
               options.SlidingExpiration = true;

               if (string.IsNullOrEmpty(Configuration["CI"]))
               {
                   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
               }
           });
        services.AddHealthChecks();
        // Initialize the ConversionsUrl
        var serviceLinkOptions = Configuration.GetSection("ServiceLink").Get<ServiceLinkOptions>();
        Links.InitializeConversionsUrl(serviceLinkOptions.ConversionsUrl);

        // Enforce HTTPS in ASP.NET Core
        // @link https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
    }

    /// <summary>
    ///    Builds Authorization policy
    ///    Ensure authenticated user and restrict roles if they are provided in configuration
    /// </summary>
    /// <returns>AuthorizationPolicyBuilder</returns>
    private AuthorizationPolicyBuilder SetupAuthorizationPolicyBuilder()
    {
        var policyBuilder = new AuthorizationPolicyBuilder();
        var allowedRoles = Configuration.GetSection("AzureAd")["AllowedRoles"];
        policyBuilder.RequireAuthenticatedUser();
        if (!string.IsNullOrWhiteSpace(allowedRoles))
        {
            policyBuilder.RequireClaim(ClaimTypes.Role, allowedRoles.Split(','));
        }

        return policyBuilder;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Ensure we do not lose X-Forwarded-* Headers when behind a Proxy
        var forwardOptions = new ForwardedHeadersOptions {
            ForwardedHeaders = ForwardedHeaders.All,
            RequireHeaderSymmetry = false
        };
        forwardOptions.KnownNetworks.Clear();
        forwardOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(forwardOptions);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Errors");
        }

        app.UseSecurityHeaders(SecurityHeadersDefinitions
           .GetHeaderPolicyCollection(env.IsDevelopment(), GetTypedConfiguration<AllowedExternalSourcesOptions>()));
        app.UseHsts();

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            HttpOnly = HttpOnlyPolicy.Always,
            Secure = CookieSecurePolicy.Always
        });

        app.UseStatusCodePagesWithReExecute("/Errors", "?statusCode={0}");

        if (!string.IsNullOrEmpty(Configuration["CI"]))
        {
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseHealthChecks("/health");

        app.UseRouting();

        app.UseSentryTracing();

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", context =>
          {
              context.Response.Redirect("home", false);
              return Task.CompletedTask;
          });
            endpoints.MapRazorPages();
            endpoints.MapControllerRoute("default", "{controller}/{action}/");
            endpoints.MapHealthChecks("/health").WithMetadata(new AllowAnonymousAttribute());
        });
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var tramsApiBase = configuration["TRAMS_API_BASE"];
        var tramsApiKey = configuration["TRAMS_API_KEY"];
        var academisationApiBase = configuration["ACADEMISATION_API_BASE"];
        var academisationApiKey = configuration["ACADEMISATION_API_KEY"];

        services.AddHttpClient("TramsApiClient", httpClient =>
        {
            httpClient.BaseAddress = new Uri(tramsApiBase);
            httpClient.DefaultRequestHeaders.Add("ApiKey", tramsApiKey);
            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "PrepareTransfers/1.0");
        });

        services.AddHttpClient("AcademisationApiClient", httpClient =>
        {
            httpClient.BaseAddress = new Uri(academisationApiBase);
            httpClient.DefaultRequestHeaders.Add("x-api-key", academisationApiKey);
            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "PrepareTransfers/1.0");
        });

        services.AddScoped<IReferenceNumberService, ReferenceNumberService>();
        services.AddScoped<ErrorService>();


        services.AddTransient<IMapper<TramsTrustSearchResult, TrustSearchResult>, TramsSearchResultMapper>();
        services.AddTransient<IMapper<TrustDto, Trust>, TramsTrustMapper>();
        services.AddTransient<IMapper<TramsEstablishment, Academy>, TramsEstablishmentMapper>();
        services.AddTransient<IMapper<EstablishmentDto, Academy>, AcademiesEstablishmentMapper>();
        services.AddTransient<IMapper<TramsProjectSummary, ProjectSearchResult>, TramsProjectSummariesMapper>();
        services.AddTransient<IMapper<AcademisationProject, Project>, AcademisationProjectMapper>();
        services.AddTransient<IMapper<TramsEducationPerformance, EducationPerformance>, TramsEducationPerformanceMapper>();
        services.AddTransient<IMapper<Project, TramsProjectUpdate>, InternalProjectToUpdateMapper>();
        services.AddTransient<ITrusts, TramsTrustsRepository>();
        services.AddTransient<IAcademies, TramsEstablishmentRepository>();
        services.AddTransient<IEducationPerformance, TramsEducationPerformanceRepository>();
        services.AddTransient<IProjects, TramsProjectsRepository>();
        services.AddTransient<ICreateProjectTemplate, CreateProjectTemplate>();
        services.AddTransient<IGetInformationForProject, GetInformationForProject>();
        services.AddTransient<IGetProjectTemplateModel, GetProjectTemplateModel>();
        services.AddTransient<ITaskListService, TaskListService>();

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IGraphClientFactory, GraphClientFactory>();
        services.AddTransient<IGraphUserService, GraphUserService>();

        services.AddScoped<ITramsHttpClient, TramsHttpClient>();
        services.AddScoped<IAcademisationHttpClient, AcademisationHttpClient>();
        services.AddScoped<IAcademyTransfersAdvisoryBoardDecisionRepository, AcademyTransfersAdvisoryBoardDecisionRepository>();

        services.AddSingleton<PerformanceDataChannel>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IAuthorizationHandler, HeaderRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();
        services.AddScoped<ICorrelationContext, CorrelationContext>();

        services.AddHostedService<PerformanceDataProcessingService>();
    }
}
