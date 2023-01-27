using Data.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Models.LegalRequirements
{
    public abstract class CommonLegalViewModel
    {
        public IList<RadioButtonViewModel> GetRadioButtons(string valueSelected, ThreeOptions? optionSelected, string name)
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = name,
                    Value = "Yes",
                    Checked = valueSelected is "Yes"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = name,
                    Value = "No",
                    Checked = valueSelected is "No"
                },
                new RadioButtonViewModel
                {
                    DisplayName = "Not Applicable",
                    Name = name,
                    Value = "NotApplicable",
                    Checked = valueSelected is "Not Applicable"
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == optionSelected.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}
