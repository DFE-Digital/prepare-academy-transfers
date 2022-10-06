using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.Projects
{
    public class TransferLegalRequirements
    {
        public ThreeOptions? TrustAgreement { get; set; }
        public ThreeOptions? DiocesanConsent { get; set; }
        public ThreeOptions? FoundationConsent { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
