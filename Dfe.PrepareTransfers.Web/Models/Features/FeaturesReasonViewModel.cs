using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Web.Models.Forms;

namespace Dfe.PrepareTransfers.Web.Models.Features
{
    public class FeaturesReasonViewModel
    {
        public bool? IsSubjectToIntervention { get; set; }
        public string MoreDetail { get; set; }
    }
}