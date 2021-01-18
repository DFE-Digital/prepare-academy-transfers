using API.Models.D365.Enums;
using Newtonsoft.Json;
using System;

namespace API.Models.Downstream.D365
{
    public class SearchProjectsD365Model
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

        public override bool Equals(object obj)
        {
            return ProjectId == ((SearchProjectsD365Model)obj).ProjectId;
        }

        public override int GetHashCode()
        {
            return ProjectId.GetHashCode();
        }
    }


}
