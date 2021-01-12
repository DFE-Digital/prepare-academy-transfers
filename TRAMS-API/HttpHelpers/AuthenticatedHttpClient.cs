using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.HttpHelpers
{
    public class AuthenticatedHttpClient : HttpClient, IAuthenticatedHttpClient
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _authority;
        private readonly string _version;
        private readonly string _url;

        public AuthenticatedHttpClient(string clientId,
                                       string clientSecret,
                                       string authority,
                                       string version,
                                       string url) : base()
        {

            _clientId = clientId;
            _clientSecret = clientSecret;
            _authority = authority;
            _version = version;
            _url = url;

            BaseAddress = new Uri($@"{_url}/api/data/{_version}/");
            DefaultRequestHeaders.Add("prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
            DefaultRequestHeaders.Add("OData-Version", "4.0");
        }


        public async Task AuthenticateAsync()
        {
            var authContext = new AuthenticationContext(_authority);
            var clientCredential = new ClientCredential(_clientId, _clientSecret);

            var result = await authContext.AcquireTokenAsync(_url, clientCredential);

            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, ByteArrayContent content)
        {
            var result = await base.PostAsync(url, content);
            return result;
        }
    }
}
