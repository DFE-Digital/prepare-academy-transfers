using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace TRAMS_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var authority = _config["D365:Authority"];
            var clientId = _config["D365:ClientId"];
            var clientSecret = _config["D365:ClientSecret"];
            var url = _config["D365:Url"];
            
            var authContext = new AuthenticationContext(authority);
            var clientCredential = new ClientCredential(clientId, clientSecret);
            var token = await authContext.AcquireTokenAsync(url, clientCredential);

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            var result = await client.GetAsync($"{url}/api/data/v9.1/accounts?$select=name&$filter=statecode eq 0");

            var resultContent = await result.Content.ReadAsStringAsync();

            return $"Request Status Code: {result.StatusCode}. First 200 chars of response are: {resultContent.Substring(0, 200)}";
        }
    }
}
