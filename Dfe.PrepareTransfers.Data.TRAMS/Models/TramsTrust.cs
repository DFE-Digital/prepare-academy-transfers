using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public class TramsTrust
    {
        public TramsTrust()
        {
            Establishments = new List<TramsEstablishment>();
            GiasData = new TramsTrustGiasData();
            IfdData = new TramsTrustIfdData();
        }
        public List<TramsEstablishment> Establishments { get; set; }
        public TramsTrustGiasData GiasData { get; set; }
        public TramsTrustIfdData IfdData { get; set; }
    }
}