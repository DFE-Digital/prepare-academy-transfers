using Frontend.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services.AzureAd
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

		public async Task<IEnumerable<Microsoft.Graph.User>> GetAllUsers()
		{
			var users = new List<Microsoft.Graph.User>();
			IGroupMembersCollectionWithReferencesPage members;

			do
			{
				members = await _client.Groups[_azureAdOptions.GroupId.ToString()].Members
					.Request()
					.GetAsync();

				users.AddRange(members.Cast<Microsoft.Graph.User>().ToList());
			}
			while (members.NextPageRequest != null);

			return users;
		}
	}
}
