namespace Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject
{
    public class TransferringAcademyGeneralInformationUpdate
    {
        public string TransferringAcademyUkprn { get; set; }
        public string PFIScheme { get; set; }
        public string PFISchemeDetails { get; set; }
        public decimal? DistanceFromAcademyToTrustHq { get; set; }
        public string DistanceFromAcademyToTrustHqDetails { get; set; }

    }
}