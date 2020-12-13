using API.HttpHelpers;
using API.Mapping;
using API.Models.D365;
using API.Models.Response;
using API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.AddControllers();

            services.AddSingleton(this.CreateHttpClient());
            ConfigureODataModelHelpers(services);

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
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddTransient(typeof(TrustsRepository));
            services.AddTransient(typeof(AcademiesRepository));
        }

        private static void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IMapper<GetTrustsD365Model, GetTrustsModel>>(r =>
                new GetTrustD365ModelToGetTrustsModelMapper());

            services.AddTransient<IMapper<GetAcademiesD365Model, GetAcademiesModel>>(r =>
                new GetAcademiesD365ModelToGetAcademiesModelMapper());
        }

        private static void ConfigureODataModelHelpers(IServiceCollection services)
        {
            services.AddTransient<IODataModelHelper<GetTrustsD365Model>>(r =>
                new ODataModelHelper<GetTrustsD365Model>());

            services.AddTransient<IODataModelHelper<GetAcademiesD365Model>>(r =>
               new ODataModelHelper<GetAcademiesD365Model>());
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
