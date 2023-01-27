using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Dfe.PrepareTransfers.Web.ExtensionMethods
{
	public static class TempDataExtensions
	{
		public static void SetNotification(this ITempDataDictionary tempData, string notificationTitle, string notificationMessage)
		{
			tempData["Success.Message"] = notificationMessage;
			tempData["Success.Title"] = notificationTitle;
		}
	}
}
