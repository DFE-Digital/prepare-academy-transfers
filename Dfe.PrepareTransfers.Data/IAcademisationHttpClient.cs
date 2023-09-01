using System.Net.Http;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Data
{
    public interface IAcademisationHttpClient
    {
        public Task<HttpResponseMessage> GetAsync(string url);

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content);

        public Task<HttpResponseMessage> PatchAsync(string url, HttpContent content);
    }
}