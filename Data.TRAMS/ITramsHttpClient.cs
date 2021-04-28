using System.Net.Http;
using System.Threading.Tasks;

namespace Data.TRAMS
{
    public interface ITramsHttpClient
    {
        public Task<HttpResponseMessage> GetAsync(string url);
    }
}