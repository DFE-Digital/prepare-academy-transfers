using Data;
using Data.Mock;
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
using Microsoft.AspNetCore.Authentication.Cookies;
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
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Localization;


namespace Frontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
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
                })
                .AddViewOptions(options => { options.HtmlHelperOptions.ClientValidationEnabled = false; });

            services.AddControllersWithViews(options => options.Filters.Add(
                    new AutoValidateAntiforgeryTokenAttribute()))
                .AddSessionStateTempDataProvider();


            services.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<FeaturesInitiatedValidator>();
                fv.DisableDataAnnotationsValidation = true;
            });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            ConfigureRedisConnection(services);

            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            ConfigureTramsRepositories(services, Configuration);

            ConfigureServiceClasses(services);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = ".ManageAnAcademyTransfer.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/home/login";
                options.Cookie.Name = ".ManageAnAcademyTransfer.Login";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                if (string.IsNullOrEmpty(Configuration["CI"]))
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }
            });

            services.AddHealthChecks();
        }

        private void ConfigureRedisConnection(IServiceCollection services)
        {
            var redisPass = "";
            var redisHost = "";
            var redisPort = "";
            var redisTls = false;

            if (!string.IsNullOrEmpty(Configuration["VCAP_SERVICES"]))
            {
                var vcapConfiguration = JObject.Parse(Configuration["VCAP_SERVICES"]);
                var redisCredentials = vcapConfiguration["redis"]?[0]?["credentials"];
                redisPass = (string)redisCredentials?["password"];
                redisHost = (string)redisCredentials?["host"];
                redisPort = (string)redisCredentials?["port"];
                redisTls = (bool)redisCredentials?["tls_enabled"];
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
                EndPoints = { $"{redisHost}:{redisPort}" },
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSentryTracing();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/");
                endpoints.MapHealthChecks("/health").WithMetadata(new AllowAnonymousAttribute());
            });
        }

        private static void ConfigureTramsRepositories(IServiceCollection services, IConfiguration configuration)
        {
            var tramsApiBase = configuration["TRAMS_API_BASE"];
            var tramsApiKey = configuration["TRAMS_API_KEY"];
            if (string.IsNullOrEmpty(tramsApiBase) || string.IsNullOrEmpty(tramsApiKey))
            {
                services.AddTransient<IAcademies, MockAcademyRepository>();
                services.AddTransient<ITrusts, MockTrustsRepository>();
                services.AddSingleton<IProjects, MockProjectRepository>();
            }
            else
            {
                services.AddSingleton(new TramsHttpClient(tramsApiBase, tramsApiKey));
                services.AddSingleton<ITramsHttpClient>(r => new TramsHttpClient(tramsApiBase, tramsApiKey));
                services.AddTransient<IMapper<TramsTrustSearchResult, TrustSearchResult>, TramsSearchResultMapper>();
                services.AddTransient<IMapper<TramsTrust, Trust>, TramsTrustMapper>();
                services.AddTransient<IMapper<TramsEstablishment, Academy>, TramsEstablishmentMapper>();
                services.AddTransient<IMapper<TramsProjectSummary, ProjectSearchResult>, TramsProjectSummariesMapper>();
                services.AddTransient<IMapper<TramsProject, Project>, TramsProjectMapper>();
                services
                    .AddTransient<IMapper<TramsEducationPerformance, EducationPerformance>,
                        TramsEducationPerformanceMapper>();
                services.AddTransient<IMapper<Project, TramsProjectUpdate>, InternalProjectToUpdateMapper>();
                services.AddTransient<ITrusts, TramsTrustsRepository>();
                services.AddTransient<IAcademies, TramsEstablishmentRepository>();
                services.AddTransient<IEducationPerformance, TramsEducationPerformanceRepository>();
                services.AddSingleton<IProjects, TramsProjectsRepository>();
            }
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            ///home/runner/work/academy-transfers-api/academy-transfers-api/Frontend.Tests/ValidatorTests/TransferDates/HtbDateValidatorTests.cs(51,0)
            ///  Frontend.Tests.ControllerTests.Projects.TransferDatesControllerTests+HtbDateTests+PostTests.GivenUrnAndFullDate_UpdatesTheProjectWithTheCorrectDate
        }

        private static void ConfigureServiceClasses(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICreateHtbDocument, CreateHtbDocument>();
            serviceCollection.AddTransient<IGetInformationForProject, GetInformationForProject>();
            serviceCollection.AddTransient<IGetHtbDocumentForProject, GetHtbDocumentForProject>();
        }
    }
}