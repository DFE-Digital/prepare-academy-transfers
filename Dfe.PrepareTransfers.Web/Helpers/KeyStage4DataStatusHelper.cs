using System;
using System.Text;

namespace Dfe.PrepareTransfers.Web.Helpers
{
	public static class KeyStage4DataStatusHelper
	{
		public static string KeyStageDataTag(DateTime date)
		{
			string status = DetermineKeyStageDataStatus(date);
			var colour = status.ToLower() switch
			{
				"revised" => "orange",
				"final" => "green",
				"provisional" => "grey",
				_ => string.Empty
			};
			return $"<td class='govuk-table__cell'><strong class='govuk-tag govuk-tag--{colour}'>{status}</strong></td>";
		}

		public static string DetermineKeyStageDataStatus(DateTime date)
		{
			// Check where and which academic year the tag is in relation too
			bool isItCurrentAcademicYear = date.Month < 9 && date.Year == DateTime.Now.Year ||
			                               date.Month >= 9 && date.Year == DateTime.Now.Year - 1;
			var status = isItCurrentAcademicYear switch
			{
				// Rules - KS4 – Provisional October, Revised January; Final April
				true => date.Month switch
				{
					>= 9 => "Provisional",
					<= 4 => "Revised",
					> 4 => "Final"
				},
				false => "Final"
			};
			return status;
		}

		public static string KeyStageDataRow()
		{
			StringBuilder rowString = new("<tr class='govuk-table__row'>");
			rowString.Append("<th scope='row' class='govuk-table__header'>Status</th>");
			rowString.Append(KeyStageDataTag(DateTime.Now));
			rowString.Append(KeyStageDataTag(DateTime.Now.AddYears(-1)));
			rowString.Append(KeyStageDataTag(DateTime.Now.AddYears(-2)));
			rowString.Append("</tr>");
			return rowString.ToString();
		}
	}
}


