using System;
using Data;
using Data.Mock;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.TRAMS;
using Data.TRAMS.Mappers.Request;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Data.TRAMS.Models.EducationPerformance;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

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
            services.AddRazorPages()
                .AddViewOptions(options =>
                {
                    options.HtmlHelperOptions.ClientValidationEnabled = false;
                });
            services.AddControllersWithViews(options => options.Filters.Add(
                new AutoValidateAntiforgeryTokenAttribute()))
                .AddSessionStateTempDataProvider();
            
            var vcapConfiguration = JObject.Parse(Configuration["VCAP_SERVICES"]);
            var redisCredentials = vcapConfiguration["redis"]?[0]?["credentials"];
            var redisConfigurationOptions = new ConfigurationOptions()
            {
                Password = (string) redisCredentials?["password"],
                EndPoints = {$"{redisCredentials?["host"]}:{redisCredentials?["port"]}"},
                Ssl = (bool) redisCredentials?["tls_enabled"]
            };
            var redisConnection = ConnectionMultiplexer.Connect(redisConfigurationOptions);

            services.AddStackExchangeRedisCache(
                options => { options.ConfigurationOptions = redisConfigurationOptions; });
            services.AddDataProtection().PersistKeysToStackExchangeRedis(redisConnection, "DataProtectionKeys");

            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            ConfigureTramsRepositories(services, Configuration);

            ConfigureServiceClasses(services);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.Name = ".AcademyTransfers.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/home/login";
                options.Cookie.Name = ".AcademyTransfers.Login";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/");
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
                services.AddTransient<IMapper<TramsEducationPerformance, EducationPerformance>, TramsEducationPerformanceMapper>();
                services.AddTransient<IMapper<Project, TramsProjectUpdate>, InternalProjectToUpdateMapper>();
                services.AddTransient<ITrusts, TramsTrustsRepository>();
                services.AddTransient<IAcademies, TramsEstablishmentRepository>();
                services.AddTransient<IEducationPerformance, TramsEducationPerformanceRepository>();
                services.AddSingleton<IProjects, TramsProjectsRepository>();
            }
        }

        private static void ConfigureServiceClasses(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICreateHtbDocument, CreateHtbDocument>();
            serviceCollection.AddTransient<IGetInformationForProject, GetInformationForProject>();
            serviceCollection.AddTransient<IGetHtbDocumentForProject, GetHtbDocumentForProject>();
        }
    }
}