using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Frontend.Helpers;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models
{
    public class TransferDatesViewModel : ProjectViewModel
    {
        public readonly FormErrorsViewModel FormErrors;

        public TransferDatesViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }

        public DateInputViewModel TransferFirstDiscussed => DateInputForField(Project.Dates.FirstDiscussed);
        public DateInputViewModel TargetDate => DateInputForField(Project.Dates.Target);
        public DateInputViewModel HtbDate => DateInputForField(Project.Dates.Htb);

        private static DateInputViewModel DateInputForField(string transferDatesFirstDiscussed)
        {
            var splitDate = DatesHelper.DateStringToDayMonthYear(transferDatesFirstDiscussed);
            return new DateInputViewModel {Day = splitDate[0], Month = splitDate[1], Year = splitDate[2]};
        }
    }
}