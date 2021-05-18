using Frontend.Models.Forms;

namespace Frontend.Models
{
    public class BenefitsViewModel : ProjectViewModel
    {
        public readonly FormErrorsViewModel FormErrors;

        public BenefitsViewModel()
        {
            FormErrors = new FormErrorsViewModel();
        }
    }
}