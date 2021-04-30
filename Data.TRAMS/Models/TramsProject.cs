using Newtonsoft.Json;

namespace Data.TRAMS.Models
{
    public class TramsProject
    {
        [JsonProperty("project_urn")] public string ProjectUrn { get; set; }
        [JsonProperty("outgoing_trust_urn")] public string OutgoingTrustUrn { get; set; }

        [JsonProperty("transferring_academies")]
        public TransferringAcademy TransferringAcademies { get; set; }

        [JsonProperty("features")] public AcademyTransferProjectFeatures Features { get; set; }
        [JsonProperty("state")] public string State { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }

    public class AcademyTransferProjectFeatures
    {
        [JsonProperty("who_initiated_the_transfer")]
        public string WhoInitiatedTheTransfer { get; set; }

        [JsonProperty("rdd_or_esfa_intervention")]
        public bool RddOrEsfaIntervention { get; set; }

        [JsonProperty("rdd_or_esfa_intervention_detail")]
        public string RddOrEsfaInterventionDetail { get; set; }

        [JsonProperty("type_of_transfer")] public string TypeOfTransfer { get; set; }
    }

    public class TransferringAcademy
    {
        [JsonProperty("outgoing_academy_urn")] public string OutgoingAcademyUrn { get; set; }
        [JsonProperty("incoming_trust_urn")] public string IncomingTrustUrn { get; set; }
    }
}