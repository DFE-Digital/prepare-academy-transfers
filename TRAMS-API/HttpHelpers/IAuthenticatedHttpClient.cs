using System.Net.Http;
using System.Threading.Tasks;

namespace API.HttpHelpers
{
    public interface IAuthenticatedHttpClient
    {
        public Task<HttpResponseMessage> GetAsync(string url);

        public Task<HttpResponseMessage> PostAsync(string url, ByteArrayContent content);

        public Task<HttpResponseMessage> PatchAsync(string url, HttpContent content);
    }
}
