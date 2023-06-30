using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Integration.Tests.Pages.Projects.ProjectAssignment
{
	public class TestUserRepository : IUserRepository
	{
		public Task<IEnumerable<User>> GetAllUsers()
		{
			var toReturn = new List<User>();
			toReturn.Add(new User("51bbe15b-4775-4000-8383-dc4173cf9c07", "bob1.@education.gov.uk", "Bob 1"));
			toReturn.Add(new User("2d6c4908-3c68-45bf-8cbc-4a034343dcdc", "bob2.@education.gov.uk", "Bob 2"));
			toReturn.Add(new User("a41e2807-6cac-494e-81de-f1b586b13b04", "bob3@education.gov.uk", "Bob 3"));

			return Task.FromResult(toReturn.AsEnumerable());
		}
	}
}
