using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http.Headers;

namespace Dfe.PrepareTransfers.Web.Services.AzureAd
{
    public class GraphClientFactory : IGraphClientFactory
	{
		private readonly AzureAdOptions _azureAdOptions;

		public GraphClientFactory(IOptions<AzureAdOptions> azureAdOptions)
		{
			_azureAdOptions = azureAdOptions.Value;
		}

		public GraphServiceClient Create()
		{
			var app = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId.ToString())
				.WithClientSecret(_azureAdOptions.ClientSecret)
				.WithAuthority(new Uri(_azureAdOptions.Authority))
				.Build();

			DelegateAuthenticationProvider provider = new DelegateAuthenticationProvider(async requestMessage =>
			{
				var result = await app.AcquireTokenForClient(_azureAdOptions.Scopes).ExecuteAsync();
				requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
			});

			return new GraphServiceClient($"{_azureAdOptions.ApiUrl}/V1.0/", provider);
		}

	}
}
