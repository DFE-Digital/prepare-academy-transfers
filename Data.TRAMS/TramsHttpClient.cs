using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Data.TRAMS
{
    public class TramsHttpClient : HttpClient, ITramsHttpClient
    {
        public TramsHttpClient(string url)
        {
            BaseAddress = new Uri(url);
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