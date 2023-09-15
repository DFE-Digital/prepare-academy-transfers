using System.Net.Http;
using System.Threading.Tasks;
using Dfe.Academisation.CorrelationIdMiddleware;

namespace Dfe.PrepareTransfers.Data.TRAMS
{
    public class AcademisationHttpClient : HttpClient, IAcademisationHttpClient
    {
        private readonly HttpClient _httpClient;

        public AcademisationHttpClient(IHttpClientFactory httpClientFactory, ICorrelationContext correlationContext)
        {
            _httpClient = httpClientFactory.CreateClient("AcademisationApiClient");
            _httpClient.DefaultRequestHeaders.Add(Dfe.Academisation.CorrelationIdMiddleware.Keys.HeaderKey, correlationContext.CorrelationId.ToString());
        }

        public new async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return response;
        }

        public new async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            var response = await _httpClient.PostAsync(url, content);
            return response;
        }

        public new async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            var response = await _httpClient.PutAsync(url, content);
            return response;
        }
    }
}