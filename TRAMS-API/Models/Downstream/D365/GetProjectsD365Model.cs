using API.Models.D365;
using API.Models.D365.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace API.Models.Downstream.D365
{
    public class GetProjectsD365Model : BaseD365Model
    {
        [JsonProperty("sip_academytransfersprojectid")]
        public Guid ProjectId { get; set; }

        [JsonProperty("sip_projectname")]
        public string ProjectName { get; set; }

        [JsonProperty("sip_projectinitiatorfullname")]
        public string ProjectInitiatorFullName { get; set; }

        [JsonProperty("sip_projectinitiatoruniqueid")]
        public string ProjectInitiatorUid { get; set; }

        [JsonProperty("sip_projectstatus")]
        public ProjectStatusEnum ProjectStatus { get; set; }

        [JsonProperty("sip_sip_academytransfersproject_sip_academytransfersprojectacademy_AcademyTransfersProjectId")]
        public List<AcademyTransfersProjectAcademy> Academies { get; set; }

        [JsonProperty("sip_sip_academytransfersproject_sip_academytransfersprojecttrust_ATProjectId")]
        public List<ProjectTrust> Trusts { get; set; }
    }

    public class AcademyTransfersProjectAcademy : BaseD365Model
    {
        [JsonProperty("sip_academytransfersprojectacademyid")]
        public Guid AcademyTransfersProjectAcademyId { get; set; }

        [JsonProperty("_sip_atprojectid_value")]
        public Guid ProjectId { get; set; }

        [JsonProperty("_sip_academyid_value")]
        public Guid AcademyId { get; set; }

        [JsonProperty("_sip_academyid_value@OData.Community.Display.V1.FormattedValue")]
        public string AcademyName { get; set; }

        [JsonProperty("sip_esfainterventionreason")]
        public string EsfaInterventionReasons { get; set; }

        [JsonProperty("sip_rddorrscinterventionreasonsexplanation")]
        public string EsfaInterventionReasonsExplained { get; set; }

        [JsonProperty("sip_rddorrscinterventionreasons")]
        public string RddOrRscInterventionReasons { get; set; }

        [JsonProperty("sip_esfainterventionreasonexplanation")]
        public string RddOrRscInterventionReasonsExplained { get; set; }

        [JsonProperty("sip_sip_academytransfersprojectacademy_sip_atacademytrust_ATProjectAcademyId")]
        public List<ProjectAcademyTrust> ProjectAcademyTrusts { get; set; }
    }

    public class ProjectAcademyTrust : BaseD365Model
    {
        [JsonProperty("sip_atacademytrustid")]
        public Guid ProjectAcademyTrustId { get; set; }

        [JsonProperty("_sip_atprojectacademyid_value")]
        public Guid ProjectAcademyId { get; set; }

        [JsonProperty("_sip_trustid_value")]
        public Guid TrustId { get; set; }

        [JsonProperty("_sip_trustid_value@OData.Community.Display.V1.FormattedValue")]
        public string TrustName { get; set; }
    }

    public class ProjectTrust : BaseD365Model
    {
        [JsonProperty("sip_academytransfersprojecttrustid")]
        public Guid ProjectTrustId { get; set; }

        [JsonProperty("_sip_atprojectid_value")]
        public Guid ProjectId { get; set; }

        [JsonProperty("_sip_trustid_value")]
        public Guid TrustId { get; set; }

        [JsonProperty("_sip_trustid_value@OData.Community.Display.V1.FormattedValue")]
        public string TrustName { get; set; }
    }
}
