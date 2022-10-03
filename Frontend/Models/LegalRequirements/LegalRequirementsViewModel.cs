
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
            
            public readonly ThreeOptions? TrustAgreement;
            public readonly ThreeOptions? DiocesanConsent;
            public readonly ThreeOptions? FoundationConsent;
            

            public LegalRequirementsViewModel(ThreeOptions? trustAgreement,
                ThreeOptions? diocesanConsent,
                ThreeOptions? foundationConsent,
                string projectUrn)
            {
                TrustAgreement = trustAgreement;
                DiocesanConsent = diocesanConsent;
                FoundationConsent = foundationConsent;
                Urn = projectUrn;
            }

        }
    }