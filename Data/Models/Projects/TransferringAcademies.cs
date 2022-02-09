using Helpers;

namespace Data.Models.Projects
{
    public class TransferringAcademies
    {
        public string IncomingTrustName { get; set; }
        public string IncomingTrustNameInTitleCase => IncomingTrustName.ToTitleCase();
        public string IncomingTrustUkprn { get; set; }
        public string IncomingTrustLeadRscRegion { get; set; }
        public string OutgoingAcademyName { get; set; }
        public string OutgoingAcademyUkprn { get; set; }
        public string OutgoingAcademyUrn { get; set; }
    }
}