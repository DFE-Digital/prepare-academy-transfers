using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Frontend.ExtensionMethods
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
