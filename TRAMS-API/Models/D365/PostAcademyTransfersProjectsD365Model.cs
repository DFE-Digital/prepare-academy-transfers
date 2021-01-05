using Newtonsoft.Json;
using System.Collections.Generic;

namespace API.Models.D365
{
    public class PostAcademyTransfersProjectsD365Model
    {
        [JsonProperty("sip_projectinitiatorfullname")]
        public string ProjectInitiatorFullName { get; set; }

        [JsonProperty("sip_projectinitiatoruniqueid")]
        public string ProjectInitiatorUid { get; set; }

        [JsonProperty("sip_projectstatus")]
        public ProjectStatusEnum ProjectStatus { get; set; }

        [JsonProperty("sip_sip_academytransfersproject_sip_academytransfersprojectacademy_AcademyTransfersProjectId")]
        public List<PostAcademyTransfersProjectAcademyD365Model> Academies { get; set; }

        [JsonProperty("sip_sip_academytransfersproject_sip_academytransfersprojecttrust_ATProjectId")]
        public List<PostAcademyTransfersProjectTrustD365Model> Trusts { get; set; }
    }

    public class PostAcademyTransfersProjectAcademyD365Model
    {
        [JsonProperty("sip_AcademyId@odata.bind")]
        public string AcademyId { get; set; }

        [JsonProperty("sip_esfainterventionreason")]
        public string EsfaInterventionReasons { get; set; }

        [JsonProperty("sip_rddorrscinterventionreasonsexplanation")]
        public string EsfaInterventionReasonsExplained { get; set; }

        [JsonProperty("sip_rddorrscinterventionreasons")]
        public string RddOrRscInterventionReasons { get; set; }

        [JsonProperty("sip_esfainterventionreasonexplanation")]
        public string RddOrRscInterventionReasonsExplained { get; set; }

        [JsonProperty("sip_sip_academytransfersprojectacademy_sip_atacademytrust_ATProjectAcademyId")]
        public List<PostAcademyTransfersProjectAcademyTrustD365Model> Trusts { get; set;}
    }

    public class PostAcademyTransfersProjectAcademyTrustD365Model
    {
        [JsonProperty("sip_TrustId@odata.bind")]
        public string TrustId { get; set; }
    }

    public class PostAcademyTransfersProjectTrustD365Model
    {
        [JsonProperty("sip_TrustId@odata.bind")]
        public string TrustId { get; set; }
    }

    public enum ProjectStatusEnum
    {
        InProgress = 596500000,
        Completed = 596500001
    }

    public enum EsfaInterventionReasonEnum
    {
        GovernanceConcerns = 596500000,
        FinanceConcerns = 596500001,
        IrregularityConcerns = 596500002,
        SafeguardingConcerns = 596500003
    }

    public enum RddOrRscInterventionReasonEnum
    {
        TerminationWarningNotice = 596500000,
        RSCMindedToTerminateNotice = 596500001,
        OfstedInadequateRating = 596500002
    }
}