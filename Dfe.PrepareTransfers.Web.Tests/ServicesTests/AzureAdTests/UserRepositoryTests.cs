using AutoFixture;
using Dfe.PrepareTransfers.Web.Services.AzureAd;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Dfe.PrepareTransfers.Helpers;
using Microsoft.Graph;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ServicesTests.AzureAdTests
{
	public class UserRepositoryTests
	{
		private readonly Fixture _fixture = new Fixture();

		[Fact]
		public async Task GetAllUsers_ReturnsUsers()
		{
			var users = GenerateUsers(20);
			var graphUserService = new Mock<IGraphUserService>();
			graphUserService.Setup(m => m.GetAllUsers()).ReturnsAsync(users);

			var sut = new UserRepository(graphUserService.Object);

			var result = (await sut.GetAllUsers()).ToList();

			Assert.Equivalent(users.Select(u => new Models.User(u.Id, u.Mail, $"{u.GivenName} {u.Surname.ToTitleCase()}")), result);
		}

		[Fact]
		public async Task SearchUsers_EmptySearch_ReturnsEmpty()
		{
			var sut = new UserRepository(Mock.Of<IGraphUserService>());
			var result = (await sut.GetAllUsers());

			Assert.Empty(result);
		}

		private List<User> GenerateUsers(int count)
		{
			var users = new List<User>();

			for (int i = 0; i < count; i++)
			{
				users.Add(
					new User { GivenName = _fixture.Create<string>(), Surname = _fixture.Create<string>(), Mail = _fixture.Create<string>(), Id = _fixture.Create<string>() }
					);
			}

			return users;
		}
	}
}
