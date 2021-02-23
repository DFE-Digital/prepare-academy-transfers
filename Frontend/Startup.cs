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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddDistributedRedisCache(options => { options.Configuration = Configuration["REDIS_URL"]; });

            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

            services.AddSingleton(CreateHttpClient());
            services.AddSingleton<IAuthenticatedHttpClient>(r => CreateHttpClient());
            services.AddTransient<IMapper<GetTrustsD365Model, GetTrustsModel>, GetTrustsReponseMapper>();

            ConfigureRepositories(services);
            ConfigureHelpers(services);
            ConfigureMappers(services);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.Name = ".AcademyTransfers.Session";
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

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddTransient<ITrustsRepository, TrustsRepository>();
            services.AddTransient<IAcademiesRepository, AcademiesRepository>();
            services.AddTransient<IProjectsRepository, ProjectsRepository>();
        }

        private static void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IMapper<GetTrustsD365Model, GetTrustsModel>,
                GetTrustsReponseMapper>();

            services.AddTransient<IMapper<GetAcademiesD365Model, GetAcademiesModel>,
                GetAcademiesResponseMapper>();

            services.AddTransient<IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model>,
                PutProjectAcademiesRequestMapper>();

            services.AddTransient<IMapper<PostProjectsAcademiesModel, PostAcademyTransfersProjectAcademyD365Model>,
                PostProjectAcademiesRequestMapper>();

            services.AddTransient<IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model>,
                PostProjectsRequestMapper>();

            services.AddTransient<IMapper<AcademyTransfersProjectAcademy, GetProjectsAcademyResponseModel>,
                GetProjectAcademiesResponseMapper>();

            services.AddTransient<IMapper<GetProjectsD365Model, GetProjectsResponseModel>,
                GetProjectsResponseMapper>();

            services.AddTransient<IMapper<SearchProjectsD365Model, SearchProjectsModel>,
                SearchProjectsItemMapper>();

            services.AddTransient<IMapper<SearchProjectsD365PageModel, SearchProjectsPageModel>,
                SearchProjectsPageResponseMapper>();
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