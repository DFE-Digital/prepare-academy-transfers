using System;
using API.HttpHelpers;
using API.Mapping;
using API.Mapping.Request;
using API.Mapping.Response;
using API.Models.Downstream.D365;
using API.Models.Upstream.Request;
using API.Models.Upstream.Response;
using API.ODataHelpers;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Wrappers;
using Data;
using Data.Mock;
using Data.Models;
using Data.TRAMS;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
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
            services.AddControllersWithViews().AddSessionStateTempDataProvider();
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

            services.AddSingleton(CreateHttpClient());
            services.AddSingleton<IAuthenticatedHttpClient>(r => CreateHttpClient());


            if (string.IsNullOrEmpty(Configuration["USE_TRAMS_REPOSITORIES"]))
            {
                ConfigureDynamicsRepositories(services);
            }
            else
            {
                ConfigureTramsRepositories(services, Configuration);
            }

            ConfigureHelpers(services);
            ConfigureMappers(services);
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
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/");
            });
        }

        private static void ConfigureDynamicsRepositories(IServiceCollection services)
        {
            services.AddTransient<IProjects, DynamicsProjectWrapper>();
            services.AddTransient<IAcademies, MockAcademyRepository>();
            services.AddTransient<ITrusts, MockTrustsRepository>();
            services.AddTransient<ITrustsRepository, TrustsDynamicsRepository>();
            services.AddTransient<IAcademiesRepository, AcademiesDynamicsRepository>();
            services.AddTransient<IProjectsRepository, ProjectsDynamicsRepository>();
        }

        private static void ConfigureTramsRepositories(IServiceCollection services, IConfiguration configuration)
        {
            var tramsApiBase = configuration["TRAMS_API_BASE"];
            var tramsApiKey = configuration["TRAMS_API_KEY"];
            if (string.IsNullOrEmpty(tramsApiBase) || string.IsNullOrEmpty(tramsApiKey))
            {
                services.AddTransient<IAcademies, MockAcademyRepository>();
                services.AddTransient<ITrusts, MockTrustsRepository>();
            }
            else
            {
                services.AddSingleton(new TramsHttpClient(tramsApiBase, tramsApiKey));
                services.AddSingleton<ITramsHttpClient>(r => new TramsHttpClient(tramsApiBase, tramsApiBase));
                services.AddTransient<IMapper<TramsTrustSearchResult, TrustSearchResult>, TramsSearchResultMapper>();
                services.AddTransient<IMapper<TramsTrust, Trust>, TramsTrustMapper>();
                services.AddTransient<IMapper<TramsAcademy, Academy>, TramsAcademyMapper>();
                services.AddTransient<ITrusts, TramsTrustsRepository>();
                services.AddTransient<IAcademies, TramsAcademiesRepository>();
            }
        }

        private static void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IDynamicsMapper<GetTrustsD365Model, GetTrustsModel>,
                GetTrustsReponseDynamicsMapper>();

            services.AddTransient<IDynamicsMapper<GetAcademiesD365Model, GetAcademiesModel>,
                GetAcademiesResponseDynamicsMapper>();

            services.AddTransient<IDynamicsMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model>,
                PutProjectAcademiesRequestDynamicsMapper>();

            services
                .AddTransient<IDynamicsMapper<PostProjectsAcademiesModel, PostAcademyTransfersProjectAcademyD365Model>,
                    PostProjectAcademiesRequestDynamicsMapper>();

            services.AddTransient<IDynamicsMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>,
                PostProjectsRequestDynamicsMapper>();

            services.AddTransient<IDynamicsMapper<AcademyTransfersProjectAcademy, GetProjectsAcademyResponseModel>,
                GetProjectAcademiesResponseDynamicsMapper>();

            services.AddTransient<IDynamicsMapper<GetProjectsD365Model, GetProjectsResponseModel>,
                GetProjectsResponseDynamicsMapper>();

            services.AddTransient<IDynamicsMapper<SearchProjectsD365Model, SearchProjectsModel>,
                SearchProjectsItemDynamicsMapper>();

            services.AddTransient<IDynamicsMapper<SearchProjectsD365PageModel, SearchProjectsPageModel>,
                SearchProjectsPageResponseDynamicsMapper>();
        }

        private static void ConfigureHelpers(IServiceCollection services)
        {
            services.AddTransient(typeof(ID365ModelHelper<>), typeof(D365ModelHelper<>));
            services.AddTransient(typeof(IOdataUrlBuilder<>), typeof(ODataUrlBuilder<>));

            services.AddTransient<IRepositoryErrorResultHandler, RepositoryErrorResultHandler>();

            services.AddTransient<IFetchXmlSanitizer, FetchXmlSanitizer>();

            services.AddTransient<IEstablishmentNameFormatter, EstablishmentNameFormatter>();

            services.AddTransient<IODataSanitizer, ODataSanitizer>();
        }

        private static void ConfigureServiceClasses(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICreateHtbDocument, CreateHtbDocument>();
            serviceCollection.AddTransient<IGetInformationForProject, GetInformationForProject>();
        }

        private AuthenticatedHttpClient CreateHttpClient()
        {
            var authority = Configuration["D365:Authority"];
            var clientId = Configuration["D365:ClientId"];
            var clientSecret = Configuration["D365:ClientSecret"];
            var url = Configuration["D365:Url"];
            var version = Configuration["D365:Version"];

            var client = new AuthenticatedHttpClient(clientId, clientSecret, authority, version, url);

            return client;
        }
    }
}