//using API.Models.D365;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace API.Models.Downstream.D365
//{
//    public class GetProjectAcademiesResponseModel : BaseD365Model
//    {
//        [JsonProperty("sip_academytransfersprojectacademyid")]
//        public Guid AcademyTransfersProjectAcademyId { get; set; }

//        [JsonProperty("_sip_atprojectid_value")]
//        public Guid ProjectId { get; set; }

//        [JsonProperty("_sip_academyid_value")]
//        public Guid AcademyId { get; set; }

//        [JsonProperty("_sip_academyid_value@OData.Community.Display.V1.FormattedValue")]
//        public string AcademyName { get; set; }

//        [JsonProperty("sip_esfainterventionreason")]
//        public string EsfaInterventionReasons { get; set; }

//        [JsonProperty("sip_rddorrscinterventionreasonsexplanation")]
//        public string EsfaInterventionReasonsExplained { get; set; }

//        [JsonProperty("sip_rddorrscinterventionreasons")]
//        public string RddOrRscInterventionReasons { get; set; }

//        [JsonProperty("sip_esfainterventionreasonexplanation")]
//        public string RddOrRscInterventionReasonsExplained { get; set; }

//        [JsonProperty("sip_sip_academytransfersprojectacademy_sip_atacademytrust_ATProjectAcademyId")]
//        public List<ProjectAcademyTrust> ProjectAcademyTrusts { get; set; }
//    }

//    public class ProjectAcademyTrust : BaseD365Model
//    {
//        [JsonProperty("sip_atacademytrustid")]
//        public Guid ProjectAcademyTrustId { get; set; }

//        [JsonProperty("_sip_atprojectacademyid_value")]
//        public Guid ProjectAcademyId { get; set; }

//        [JsonProperty("_sip_trustid_value")]
//        public Guid TrustId { get; set; }

//        [JsonProperty("_sip_trustid_value@OData.Community.Display.V1.FormattedValue")]
//        public string TrustName { get; set; }
//    }
//}
