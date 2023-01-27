
using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Utils;
using global::Dfe.PrepareTransfers.Web.Models.Benefits;
using global::Helpers;
using Helpers;

namespace Dfe.PrepareTransfers.Web.Models.LegalRequirements
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