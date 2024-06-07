using Dfe.Academisation.ExtensionMethods;

namespace Dfe.PrepareTransfers.Data.Models.Projects
{
    public class TransferringAcademy
    {
        public string IncomingTrustName { get; set; }
        public string IncomingTrustNameInTitleCase => IncomingTrustName?.ToTitleCase();
        public string IncomingTrustUkprn { get; set; }
        public string OutgoingAcademyName { get; set; }
        public string OutgoingAcademyUkprn { get; set; }
        public string Region { get; set; }
        public string LocalAuthority { get; set; }
        public string LastChangedDate { get; set; }
        public string OutgoingAcademyUrn { get; set; }
        public string? PFIScheme { get; set; }
        public string? PFISchemeDetails { get; set; }
        public decimal? DistanceFromAcademyToTrustHq { get; set; }
        public string DistanceFromAcademyToTrustHqDetails{ get; set; }

        public string PupilNumbersAdditionalInformation { get; set; }
        public string LatestOfstedReportAdditionalInformation { get; set; }
        public string KeyStage2PerformanceAdditionalInformation { get; set; }
        public string KeyStage4PerformanceAdditionalInformation { get; set; }
        public string KeyStage5PerformanceAdditionalInformation { get; set; }
    }
}