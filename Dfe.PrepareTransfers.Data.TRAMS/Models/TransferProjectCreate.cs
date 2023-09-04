using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject;
using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public class TransferProjectCreate
    {
        public string OutgoingTrustUkprn { get; set; }
        public string IncomingTrustUkprn { get; set; }
        public List<string> TransferringAcademyUkprns { get; set; }

    }
}