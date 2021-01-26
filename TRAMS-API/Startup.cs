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
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

namespace TRAMS_API
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt =>
                    {
                        opt.Audience = Configuration["AAD:ResourceId"];
                        opt.Authority = $"{Configuration["AAD:Instance"]}{Configuration["AAD:TenantId"]}";
                    });

            services.AddControllers()
                    .AddFluentValidation(s =>
                    {
                        s.RegisterValidatorsFromAssemblyContaining<Startup>();
                        s.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    }); 

            services.AddSingleton(this.CreateHttpClient());
            services.AddSingleton<IAuthenticatedHttpClient>(r => this.CreateHttpClient());
            services.AddTransient<IMapper<GetTrustsD365Model, GetTrustsModel>, GetTrustsReponseMapper>();

            // Register the Swagger Generator service. This service is responsible for genrating Swagger Documents.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Academy Transfers Prototype API",
                    Version = "v1",
                    Description = "API for the Academy Transfers frontend to talk to Dynamics 365 backend (TRAMS)",
                    Contact = new OpenApiContact
                    {
                        Email = "academytransfers@education.gov.uk",
                    },
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "API.xml");
                c.IncludeXmlComments(filePath);
            });
        
            ConfigureHelpers(services);

            ConfigureMappers(services);
            ConfigureRepositories(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Academy Transfers Prototype API");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
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
