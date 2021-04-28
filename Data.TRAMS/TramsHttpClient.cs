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
    }
}