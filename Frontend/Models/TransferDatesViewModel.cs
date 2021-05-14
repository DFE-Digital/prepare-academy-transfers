namespace Frontend.Models
{
    public class TransferDatesViewModel : ProjectViewModel
    {
        public readonly FormErrorsViewModel FormErrors;

        public TransferDatesViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }
    }
}