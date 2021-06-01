using System.Collections.Generic;

namespace Data.TRAMS.Models
{
    public class TramsTrust
    {
        public List<TramsAcademy> Academies { get; set; }
        public TramsTrustGiasData GiasData { get; set; }
        public TramsTrustIfdData IfdData { get; set; }
    }
}