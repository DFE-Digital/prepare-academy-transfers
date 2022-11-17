using System;
using System.Collections.Generic;
using System.Globalization;

namespace Frontend.Services.AzureAd
{
    public class AzureAdOptions
	{
		public const string Name = "AzureAd";
		public Guid ClientId { get; set; }
		public string ClientSecret { get; set; }
		public Guid TenantId { get; set; }
		public Guid GroupId { get; set; }
		public string ApiUrl { get; set; } = "https://graph.microsoft.com/";
		public string Authority => string.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}", TenantId);
		public IEnumerable<string> Scopes => new[] { $"{ApiUrl}.default" };
	}
}
