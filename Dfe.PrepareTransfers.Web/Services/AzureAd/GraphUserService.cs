using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Services.AzureAd
{
    public class GraphUserService : IGraphUserService
	{
		private readonly GraphServiceClient _client;
		private readonly AzureAdOptions _azureAdOptions;

		public GraphUserService(IGraphClientFactory graphClientFactory, IOptions<AzureAdOptions> azureAdOptions)
		{
			_client = graphClientFactory.Create();
			_azureAdOptions = azureAdOptions.Value;
		}

		public async Task<IEnumerable<User>> GetAllUsers()
		{
			var users = new List<User>();
			var queryOptions = new List<QueryOption>()
			{
				new QueryOption("$count", "true"),
				new QueryOption("$top", "999")
			};

			var members = await _client.Groups[_azureAdOptions.GroupId.ToString()].Members
				.Request(queryOptions)
				.Header("ConsistencyLevel", "eventual")
				.Select("givenName,surname,id,mail")
				.GetAsync();

			users.AddRange(members.Cast<User>().ToList());

			return users;
		}
	}
}
