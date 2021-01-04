using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Request
{
    public class PostProjectsRequestModel
    {
        [Required]
        public string ProjectInitiatorFullName { get; set; }

        [Required]
        public string ProjectInitiatorUid { get; set; }

        [Required]
        public int ProjectStatus { get; set; }

        public List<PostProjectsAcademiesModel> ProjectAcademies { get; set;}

        public List<PostProjectsTrustsModel> ProjectTrusts { get; set; }
    }

    public class PostProjectsAcademiesModel
    {
        [Required]
        public Guid AcademyId { get; set; }

        public List<int> EsfaInterventionReasons { get; set; }

        public string EsfaInterventionReasonsExplained { get; set; }

        public List<int> RddOrRscInterventionReasons { get; set; }

        public string RddOrRscInterventionReasonsExplained { get; set; }

        public List<PostProjectsAcademiesTrustsModel> Trusts { get; set; }
    }

    public class PostProjectsAcademiesTrustsModel
    {
        [Required]
        public Guid TrustId { get; set; }
    }

    public class PostProjectsTrustsModel
    {
        [Required]
        public Guid TrustId { get; set; }
    }
}