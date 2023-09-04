using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Data.TRAMS
{
    public class AcademisationHttpClient : HttpClient, IAcademisationHttpClient
    {
        public AcademisationHttpClient(string url, string apiKey)
        {
            BaseAddress = new Uri(url);
            DefaultRequestHeaders.Add("x-api-key", apiKey);
            DefaultRequestHeaders.Add("ContentType", "application/json");
        }

        public new async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await base.GetAsync(url);
            return response;
        }

        public new async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            var response = await base.PostAsync(url, content);
            return response;
        }

        public new async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            var response = await base.PutAsync(url, content);
            return response;
        }
    }
}