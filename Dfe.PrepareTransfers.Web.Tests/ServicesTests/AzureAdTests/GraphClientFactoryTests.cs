using Dfe.PrepareTransfers.Web.Services.AzureAd;
using Microsoft.Extensions.Options;
using Moq;
using AutoFixture;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ServicesTests.AzureAdTests
{
    public class GraphClientFactoryTests
	{
		[Fact]
		public void Create_ReturnsGraphClient()
		{
			var azureAdOptions = new Fixture().Create<AzureAdOptions>();
			var options = new Mock<IOptions<AzureAdOptions>>();
			options.SetupGet(m => m.Value).Returns(azureAdOptions);
			var sut = new GraphClientFactory(options.Object);

			var client = sut.Create();

			Assert.Multiple(
				() => Assert.NotNull(client),
				() => Assert.Equal($"{azureAdOptions.ApiUrl}/V1.0", client.BaseUrl)
			);
		}
	}
}
