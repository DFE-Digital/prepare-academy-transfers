using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Frontend.Helpers;
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