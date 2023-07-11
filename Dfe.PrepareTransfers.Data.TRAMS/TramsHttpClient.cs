namespace Dfe.PrepareTransfers.Data.TRAMS
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Academisation.CorrelationIdMiddleware;

    public class TramsHttpClient : ITramsHttpClient
    {
        private readonly ICorrelationContext _correlationContext;
        private readonly HttpClient _httpClient;

        public TramsHttpClient(IHttpClientFactory httpClientFactory, ICorrelationContext correlationContext)
        {
            _httpClient = httpClientFactory.CreateClient("TramsApiClient");
            _correlationContext = correlationContext;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            _httpClient.DefaultRequestHeaders.Add(Dfe.Academisation.CorrelationIdMiddleware.Keys.HeaderKey, _correlationContext.CorrelationId.ToString());
            var response = await _httpClient.GetAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            _httpClient.DefaultRequestHeaders.Add(Dfe.Academisation.CorrelationIdMiddleware.Keys.HeaderKey, _correlationContext.CorrelationId.ToString());
            var response = await _httpClient.PostAsync(url, content);
            return response;
        }

        public async Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            _httpClient.DefaultRequestHeaders.Add(Dfe.Academisation.CorrelationIdMiddleware.Keys.HeaderKey, _correlationContext.CorrelationId.ToString());
            var response = await _httpClient.PatchAsync(url, content);
            return response;
        }
    }
}