namespace Dfe.PrepareTransfers.Data.TRAMS
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Academisation.CorrelationIdMiddleware;

    public class TramsHttpClient : ITramsHttpClient
    {
        private readonly HttpClient _httpClient;

        public TramsHttpClient(IHttpClientFactory httpClientFactory, ICorrelationContext correlationContext)
        {
            _httpClient = httpClientFactory.CreateClient("TramsApiClient");
            _httpClient.DefaultRequestHeaders.Add(Dfe.Academisation.CorrelationIdMiddleware.Keys.HeaderKey, correlationContext.CorrelationId.ToString());
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            var response = await _httpClient.PostAsync(url, content);
            return response;
        }

        public async Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            var response = await _httpClient.PatchAsync(url, content);
            return response;
        }
    }
}