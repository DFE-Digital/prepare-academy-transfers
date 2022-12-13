using System;
using System.Collections.Generic;
using System.Text;

namespace Data.TRAMS.Models.AcademyTransferProject
{
    public class AcademyTransferProjectLegalRequirements
    {
        public string IncomingTrustAgreement { get; set; }
        public string DiocesanConsent { get; set; }
        public string OutgoingTrustConsent { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
