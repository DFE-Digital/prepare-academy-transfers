namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public class TramsTrustGiasData
    {
        public TramsTrustGiasData()
        {
            GroupContactAddress = new GroupContactAddress();
        }
        
        public string CompaniesHouseNumber { get; set; }
        public GroupContactAddress GroupContactAddress { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Ukprn { get; set; }
    }
}