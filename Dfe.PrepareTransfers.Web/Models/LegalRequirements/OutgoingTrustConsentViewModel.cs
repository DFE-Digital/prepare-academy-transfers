using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Web.Models.LegalRequirements
{
    public class OutgoingTrustConsentViewModel : CommonLegalViewModel
    {
        public ThreeOptions? OutgoingTrustConsent { get; set; }
    }
}
