using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Data.TRAMS;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Request;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models.EducationPerformance;
using FluentValidation;
using FluentValidation.AspNetCore;
using Dfe.PrepareTransfers.Web.Authorization;
using Dfe.PrepareTransfers.Web.BackgroundServices;
using Dfe.PrepareTransfers.Web.Options;
using Dfe.PrepareTransfers.Web.Security;
using Dfe.PrepareTransfers.Web.Services;
using Dfe.PrepareTransfers.Web.Services.AzureAd;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Dfe.PrepareTransfers.Web.Validators.Features;
using Dfe.PrepareTransfers.Helpers;
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
            options.Conventions.AllowAnonymousToPage("/SessionTimedOut");
         })
         .AddViewOptions(options => { options.HtmlHelperOptions.ClientValidationEnabled = false; });

      services.AddControllersWithViews(options => options.Filters.Add(
            new AutoValidateAntiforgeryTokenAttribute()))
         .AddSessionStateTempDataProvider()
         .AddMicrosoftIdentityUI();


      services.Configure<ServiceLinkOptions>(GetConfigurationSection<ServiceLinkOptions>());

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

      services.AddSession(options =>
      {
         options.IdleTimeout = _authenticationExpiration;
         options.Cookie.Name = ".ManageAnAcademyTransfer.Session";
         options.Cookie.HttpOnly = true;
         options.Cookie.IsEssential = true;

         if (string.IsNullOrEmpty(Configuration["CI"])) 
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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
               options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
         });
      services.AddHealthChecks();
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
         policyBuilder.RequireClaim(ClaimTypes.Role, allowedRoles.Split(','));
      return policyBuilder;
   }

   // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
   public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
   {
      if (env.IsDevelopment())
      {
         app.UseDeveloperExceptionPage();
      }
      else
      {
         app.UseExceptionHandler("/Errors");
         // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
         app.UseHsts();
      }

      app.UseSecurityHeaders(SecurityHeadersDefinitions
         .GetHeaderPolicyCollection(env.IsDevelopment(), GetTypedConfiguration<AllowedExternalSourcesOptions>())
         .AddXssProtectionDisabled());

      app.UseCookiePolicy(new CookiePolicyOptions
      {
         HttpOnly = HttpOnlyPolicy.Always,
         Secure = CookieSecurePolicy.Always
      });

      app.UseStatusCodePagesWithReExecute("/Errors", "?statusCode={0}");

      if (!string.IsNullOrEmpty(Configuration["CI"])) app.UseHttpsRedirection();

      //For Azure AD redirect uri to remain https
      var forwardOptions = new ForwardedHeadersOptions
      {
         ForwardedHeaders = ForwardedHeaders.All,
         RequireHeaderSymmetry = false
      };
      forwardOptions.KnownNetworks.Clear();
      forwardOptions.KnownProxies.Clear();
      app.UseForwardedHeaders(forwardOptions);

      app.UseStaticFiles();

      app.UseHealthChecks("/health");

      app.UseRouting();

      app.UseSentryTracing();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSession();

      app.UseEndpoints(endpoints =>
      {
         endpoints.MapGet("/", context =>
         {
            context.Response.Redirect("project-type", false);
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

      services.AddScoped<IReferenceNumberService, ReferenceNumberService>();

      services.AddTransient<IMapper<TramsTrustSearchResult, TrustSearchResult>, TramsSearchResultMapper>();
      services.AddTransient<IMapper<TramsTrust, Trust>, TramsTrustMapper>();
      services.AddTransient<IMapper<TramsEstablishment, Academy>, TramsEstablishmentMapper>();
      services.AddTransient<IMapper<TramsProjectSummary, ProjectSearchResult>, TramsProjectSummariesMapper>();
      services.AddTransient<IMapper<TramsProject, Project>, TramsProjectMapper>();
      services
         .AddTransient<IMapper<TramsEducationPerformance, EducationPerformance>, TramsEducationPerformanceMapper>();
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

      services.AddSingleton(new AcademisationHttpClient(academisationApiBase, academisationApiKey));
      services.AddSingleton<IAcademisationHttpClient>(r => new AcademisationHttpClient(academisationApiBase,academisationApiKey));
      services.AddSingleton(new TramsHttpClient(tramsApiBase, tramsApiKey));
      services.AddSingleton<ITramsHttpClient>(r => new TramsHttpClient(tramsApiBase, tramsApiKey));
      services.AddSingleton<PerformanceDataChannel>();
      services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
      services.AddSingleton<IAuthorizationHandler, HeaderRequirementHandler>();
      services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();

      services.AddHostedService<PerformanceDataProcessingService>();
   }
}
