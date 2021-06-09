using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Data.TRAMS
{
    public class TramsHttpClient : HttpClient, ITramsHttpClient
    {
        public TramsHttpClient(string url, string apiKey)
        {
            BaseAddress = new Uri(url);
            DefaultRequestHeaders.Add("ApiKey", apiKey);
            DefaultRequestHeaders.Add("Content-Type", "application/json");
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

        public new async Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            var response = await base.PatchAsync(url, content);
            return response;
        }
    }
}