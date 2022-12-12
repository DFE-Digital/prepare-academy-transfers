
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Frontend.Utils;
using global::Frontend.Models.Benefits;
using global::Helpers;
using Helpers;

namespace Frontend.Models.LegalRequirements
{
    public class LegalRequirementsViewModel : CommonViewModel
    {
        public readonly ThreeOptions? IncomingTrustAgreement;
        public readonly ThreeOptions? DiocesanConsent;
        public readonly ThreeOptions? OutgoingTrustConsent;

        public LegalRequirementsViewModel(ThreeOptions? incomingTrustAgreement,
            ThreeOptions? diocesanConsent,
            ThreeOptions? outgoingTrustConsent,
            string projectUrn)
        {
            IncomingTrustAgreement = incomingTrustAgreement;
            DiocesanConsent = diocesanConsent;
            OutgoingTrustConsent = outgoingTrustConsent;
            Urn = projectUrn;
        }
    }
}