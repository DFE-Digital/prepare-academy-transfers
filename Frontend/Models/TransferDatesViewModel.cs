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
    }
}