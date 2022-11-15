using Frontend.Models;
using Frontend.Services.Interfaces;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services.AzureAd
{
    public class UserRepository : IUserRepository
	{
		private readonly IGraphUserService _graphUserService;

		public UserRepository(IGraphUserService graphUserService)
		{
			_graphUserService = graphUserService;
		}

		public async Task<IEnumerable<User>> GetAllUsers()
		{
			IEnumerable<Microsoft.Graph.User> users = await _graphUserService.GetAllUsers();

			return users
				.Select(u => new User(u.Id, u.Mail, $"{u.GivenName} {u.Surname.ToTitleCase()}"));
		}
	}
}
