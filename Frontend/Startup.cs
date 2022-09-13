using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.TRAMS;
using Data.TRAMS.Mappers.Request;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Data.TRAMS.Models.EducationPerformance;
using FluentValidation.AspNetCore;
using Frontend.Security;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Frontend.Validators.Features;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Frontend.Authorization;
using Frontend.BackgroundServices;
using Frontend.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace Frontend
{
    public class Startup
    {
        private readonly IHostEnvironment _hostEnvironment;

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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


            services.Configure<ServiceLinkOptions>(Configuration.GetSection(ServiceLinkOptions.Name));
            
            services.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<FeaturesReasonValidator>();
                fv.DisableDataAnnotationsValidation = true;
            });

            ConfigureRedisConnection(services);

            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

            AddServices(services, Configuration);
        
            var policyBuilder = SetupAuthorizationPolicyBuilder();
            services.AddAuthorization(options => { options.DefaultPolicy = policyBuilder.Build(); });
            services.AddSession(options =>
            {
                options.IdleTimeout =
                    TimeSpan.FromMinutes(Int32.Parse(Configuration["AuthenticationExpirationInMinutes"]));
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
                    options.Cookie.Name = "ManageAnAcademyTransfer.Login";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.ExpireTimeSpan =
                        TimeSpan.FromMinutes(int.Parse(Configuration["AuthenticationExpirationInMinutes"]));
                    options.SlidingExpiration = true;
                    if (string.IsNullOrEmpty(Configuration["CI"]))
                    {
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    }
                });
            services.AddHealthChecks();
        }

        /// <summary>
        /// Builds Authorization policy
        /// Ensure authenticated user and restrict roles if they are provided in configuration
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

        private void ConfigureRedisConnection(IServiceCollection services)
        {
            var vcapServicesDefined = !string.IsNullOrEmpty(Configuration["VCAP_SERVICES"]);
            var redisUrlDefined = !string.IsNullOrEmpty(Configuration["REDIS_URL"]);

            if (!vcapServicesDefined && !redisUrlDefined)
            {
                return;
            }

            var redisPass = "";
            var redisHost = "";
            var redisPort = "";
            var redisTls = false;

            if (!string.IsNullOrEmpty(Configuration["VCAP_SERVICES"]))
            {
                var vcapConfiguration = JObject.Parse(Configuration["VCAP_SERVICES"]);
                var redisCredentials = vcapConfiguration["redis"]?[0]?["credentials"];
                redisPass = (string) redisCredentials?["password"];
                redisHost = (string) redisCredentials?["host"];
                redisPort = (string) redisCredentials?["port"];
                redisTls = (bool) redisCredentials?["tls_enabled"];
            }
            else if (!string.IsNullOrEmpty(Configuration["REDIS_URL"]))
            {
                var redisUri = new Uri(Configuration["REDIS_URL"]);
                redisPass = redisUri.UserInfo.Split(":")[1];
                redisHost = redisUri.Host;
                redisPort = redisUri.Port.ToString();
            }

            var redisConfigurationOptions = new ConfigurationOptions()
            {
                Password = redisPass,
                EndPoints = {$"{redisHost}:{redisPort}"},
                Ssl = redisTls
            };

            var redisConnection = ConnectionMultiplexer.Connect(redisConfigurationOptions);

            services.AddStackExchangeRedisCache(
                options => { options.ConfigurationOptions = redisConfigurationOptions; });
            services.AddDataProtection().PersistKeysToStackExchangeRedis(redisConnection, "DataProtectionKeys");
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSecurityHeaders(
                SecureHeadersDefinitions.SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));

            if (!string.IsNullOrEmpty(Configuration["CI"]))
            {
                app.UseHttpsRedirection();
            }

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

            services.AddScoped<IReferenceNumberService, ReferenceNumberService>();
           
            services.AddTransient<IMapper<TramsTrustSearchResult, TrustSearchResult>, TramsSearchResultMapper>();
            services.AddTransient<IMapper<TramsTrust, Trust>, TramsTrustMapper>();
            services.AddTransient<IMapper<TramsEstablishment, Academy>, TramsEstablishmentMapper>();
            services.AddTransient<IMapper<TramsProjectSummary, ProjectSearchResult>, TramsProjectSummariesMapper>();
            services.AddTransient<IMapper<TramsProject, Project>, TramsProjectMapper>();
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
            
            services.AddSingleton(new TramsHttpClient(tramsApiBase, tramsApiKey));
            services.AddSingleton<ITramsHttpClient>(r => new TramsHttpClient(tramsApiBase, tramsApiKey));
            services.AddSingleton<PerformanceDataChannel>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IAuthorizationHandler, HeaderRequirementHandler>();
            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();
            
            services.AddHostedService<PerformanceDataProcessingService>();
        }
    }
}
