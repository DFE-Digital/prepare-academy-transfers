using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class RationaleViewModel : ProjectViewModel
    {
        public readonly FormErrorsViewModel FormErrors;

        public RationaleViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }
    }
}