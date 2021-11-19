using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Benefits
{
    public class OtherFactorsViewModel : CommonViewModel
    {
        public List<OtherFactorsItemViewModel> OtherFactorsVm { get; set; }
    }
}