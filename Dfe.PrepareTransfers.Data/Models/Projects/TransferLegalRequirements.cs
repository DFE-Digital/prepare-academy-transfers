namespace Dfe.PrepareTransfers.Data.Models.Projects
{
    public class TransferLegalRequirements
    {
        public ThreeOptions? IncomingTrustAgreement { get; set; }
        public ThreeOptions? DiocesanConsent { get; set; }
        public ThreeOptions? OutgoingTrustConsent { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
