using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject;
using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public class TransferProjectCreate
    {
        public string OutgoingTrustUkprn { get; set; }
        public string OutgoingTrustName { get; set; }
        public List<TransferringAcademy> TransferringAcademies { get; set; }
        public bool? IsFormAMat { get; set; }

    }
}