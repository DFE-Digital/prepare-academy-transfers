using System.Collections.Generic;
using System.Linq;
using Frontend.Models.Forms;

namespace Frontend.Models.Features
{
    public class FeaturesReasonViewModel : FeaturesCommonViewModel
    {
        public bool? IsSubjectToIntervention { get; set; }
        public string MoreDetail { get; set; }
        
        public List<RadioButtonViewModel> ReasonRadioButtons()
        {
            var result = new[] { true, false }.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(),
                Name = nameof(IsSubjectToIntervention),
                DisplayName = value ? "Yes" : "No",
                Checked = IsSubjectToIntervention == value
            }).ToList();

            return result;
        }
    }
}