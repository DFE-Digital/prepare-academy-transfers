using System.Collections.Generic;
using Frontend.Models.Forms;

namespace Frontend.Models.Benefits
{
    public class RisksViewModel
    {
        public List<RadioButtonViewModel> RadioButtonsYesNo
            => new List<RadioButtonViewModel>()
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = "Yes",
                    Value = "Yes"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = "No",
                    Value = "No"
                }
            };
    }
}