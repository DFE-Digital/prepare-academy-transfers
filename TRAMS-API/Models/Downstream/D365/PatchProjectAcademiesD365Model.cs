using Newtonsoft.Json;

namespace API.Models.Downstream.D365
{
    public class PatchProjectAcademiesD365Model
    {
        [JsonProperty("sip_AcademyId@odata.bind")]
        public string AcademyId { get; set; }

        [JsonProperty("sip_esfainterventionreason")]
        public string EsfaInterventionReasons { get; set; }

        [JsonProperty("sip_esfainterventionreasonexplanation")]
        public string EsfaInterventionReasonsExplained { get; set; }

        [JsonProperty("sip_rddorrscinterventionreasons")]
        public string RddOrRscInterventionReasons { get; set; }

        [JsonProperty("sip_rddorrscinterventionreasonsexplanation")]
        public string RddOrRscInterventionReasonsExplained { get; set; }
    }
}
