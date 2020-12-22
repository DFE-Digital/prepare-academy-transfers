using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace API.Models.D365
{
    public class GetAcademyTransfersProjectsD365Model : BaseD365Model
    {
        [JsonProperty("sip_academytransfersprojectid")]
        public Guid ProjectId { get; set; }

        [JsonProperty("sip_projectinitiatorfullname")]
        public string ProjectInitiatorFullName { get; set; }

        [JsonProperty("sip_projectinitiatoruniqueid")]
        public string ProjectInitiatorUid { get; set; }

        [JsonProperty("sip_projectname")]
        public string ProjectName { get; set; }

        [JsonProperty("sip_projectstatus")]
        public int ProjectStatus { get; set; }

        [JsonProperty("sip_sip_academytransfersproject_sip_academytransfersprojectacademy_AcademyTransfersProjectId")]
        public List<AcademyTransfersProjectAcademy> Academies { get; set; }
    }

    public class AcademyTransfersProjectAcademy : BaseD365Model
    {
        [JsonProperty("sip_academytransfersprojectacademyid")]
        public Guid AcademyTransfersProjectAcademyId { get; set; }

        [JsonProperty("_sip_atprojectid_value")]
        public Guid ProjectId { get; set; }

        [JsonProperty("_sip_academyid_value")]
        public Guid AcademyId { get; set; }

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
        public Guid Id { get; set; }
    }
}
