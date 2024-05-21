using System;
using System.Collections.Generic;
using System.Text;

namespace Dfe.PrepareTransfers.Web.Helpers
{
    public static class KeyStage4DataStatusHelper
    {
        public static string KeyStageDataTag(DateTime date)
        {
            string status = DetermineKeyStageDataStatus(date, KeyStages.KS4);
            var colour = status.ToLower() switch
            {
                "revised" => "orange",
                "final" => "green",
                "provisional" => "grey",
                _ => string.Empty
            };
            return $"<td class='govuk-table__cell'><strong class='govuk-tag govuk-tag--{colour}'>{status}</strong></td>";
        }
        public enum StatusType
        {
            Provisional,
            Revised,
            Final
        }
        public enum StatusColour
        {
            Grey,
            Orange,
            Green
        }
        public enum KeyStages
        {
            KS2,
            KS4,
            KS5
        }

        private static readonly Dictionary<StatusType, (StatusColour Colour, string Description)> StatusMap = new()
        {
            { StatusType.Provisional, (StatusColour.Grey, StatusType.Provisional.ToString()) },
            { StatusType.Revised, (StatusColour.Orange, StatusType.Revised.ToString()) },
            { StatusType.Final, (StatusColour.Green, StatusType.Final.ToString()) },
        };
        public static string DetermineKeyStageDataStatus(DateTime date, KeyStages keyStage = KeyStages.KS4)
        {

            bool isItCurrentAcademicYear =
                (date.Month < 9 && date.Year == DateTime.Now.Year) ||
                (date.Month >= 9 && date.Year == DateTime.Now.Year - 1);

            bool isItLastAcademicYear =
                (date.Month < 9 && date.Year == DateTime.Now.Year - 1) ||
                (date.Month >= 9 && date.Year == DateTime.Now.Year - 2);

            StatusType statusType = StatusType.Final;

            if (isItCurrentAcademicYear)
            {
                statusType = StatusType.Provisional;
            }
            if (isItLastAcademicYear)
            {
                statusType = DetermineStatusType(date, keyStage);
            }

            return statusType.ToString();
        }
        private static StatusType DetermineStatusType(DateTime date, KeyStages keyStage)
        {
            return keyStage switch
            {
                KeyStages.KS2 => date.Month switch
                {
                    >= 9 => StatusType.Provisional,
                    <= 3 => StatusType.Revised,
                    _ => StatusType.Final
                },
                KeyStages.KS4 or KeyStages.KS5 => date.Month switch
                {
                    >= 9 => StatusType.Provisional,
                    <= 4 => StatusType.Revised,
                    _ => StatusType.Final
                },
                _ => throw new ArgumentException("Invalid key stage")
            };
        }
        public static string KeyStageDataRow(string date)
        {
            var resultDate = ConvertToDateTime(date);
            StringBuilder rowString = new("<tr class='govuk-table__row'>");
            rowString.Append("<th scope='row' class='govuk-table__header'>Status</th>");
            rowString.Append(KeyStageDataTag(resultDate));
            rowString.Append(KeyStageDataTag(resultDate.AddYears(-1)));
            rowString.Append(KeyStageDataTag(resultDate.AddYears(-2)));
            rowString.Append("</tr>");
            return rowString.ToString();
        }
        static DateTime ConvertToDateTime(string input)
        {
            string[] parts = input.Split(new string[] { "-" }, StringSplitOptions.None);

            if (parts.Length == 2 && int.TryParse(parts[1], out int endYear))
            {
                return new DateTime(endYear, 8, 31); // Last day of Aug to mark end of academic year
            }
            // Default to current year if the year isn't in the expected value
            return DateTime.UtcNow;
        }
    }
}


