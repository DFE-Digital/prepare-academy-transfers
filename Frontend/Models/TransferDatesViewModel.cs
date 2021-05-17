using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Frontend.Helpers;
using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class TransferDatesViewModel : ProjectViewModel
    {
        public readonly FormErrorsViewModel FormErrors;

        public TransferDatesViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }

        public DateInputViewModel TransferFirstDiscussed => DateInputForField(Project.TransferDates.FirstDiscussed);
        public DateInputViewModel TargetDate => DateInputForField(Project.TransferDates.Target);

        private static DateInputViewModel DateInputForField(string transferDatesFirstDiscussed)
        {
            var splitDate = DatesHelper.DateStringToDayMonthYear(transferDatesFirstDiscussed);
            return new DateInputViewModel() {Day = splitDate[0], Month = splitDate[1], Year = splitDate[2]};
        }

        public static List<RadioButtonViewModel> PotentialHtbDates(string startDateString)
        {
            DateTime.TryParseExact(startDateString, "dd/MM/yyyy", null, DateTimeStyles.None, out var date);
            var htbDates = new List<DateTime>();
            for (var i = 1; i <= 12; i++)
            {
                date = date.AddMonths(1);
                date = new DateTime(date.Year, date.Month, 1);
                if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    date = date.AddDays(2);
                }

                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    date = date.AddDays(1);
                }

                htbDates.Add(date);
            }

            return htbDates.Select(htbDate => new RadioButtonViewModel
            {
                Name = "htbDate", Value = htbDate.ToString("dd/MM/yyyy"),
                DisplayName = htbDate.ToString("d MMMM yyyy")
            }).ToList();
        }
    }
}