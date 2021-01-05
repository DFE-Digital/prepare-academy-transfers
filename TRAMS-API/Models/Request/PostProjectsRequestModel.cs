using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Request
{
    public class PostProjectsRequestModel
    {
        /// <summary>
        /// The Project Initiator's Full Name. Mandatory.
        /// </summary>
        /// <example>Joe Bloggs</example>
        [Required]
        public string ProjectInitiatorFullName { get; set; }

        /// <summary>
        /// The unique id of the project initiator. Mandatory.
        /// </summary>
        /// <example>joe.blogs@email.com</example>
        [Required]
        public string ProjectInitiatorUid { get; set; }

        /// <summary>
        /// The Project Status Code. Mandatory.
        /// 1 - In Progress
        /// 2 - Completed
        /// </summary>
        /// <example>1</example>
        [Required]
        public int ProjectStatus { get; set; }

        /// <summary>
        /// An array of outbound academies identified for the project. Optional.
        /// </summary>
        public List<PostProjectsAcademiesModel> ProjectAcademies { get; set;}

        /// <summary>
        /// An array of inbout trusts identified for the project. Optional.
        /// </summary>
        public List<PostProjectsTrustsModel> ProjectTrusts { get; set; }
    }

    public class PostProjectsAcademiesModel
    {
        /// <summary>
        /// The ID of the academy in TRAMS. Mandatory.
        /// </summary>
        [Required]
        public Guid AcademyId { get; set; }

        /// <summary>
        /// An array of ESFA Intervention Reasons. Optional.
        /// 1 - Governance Concerns
        /// 2 - Finance Concerns
        /// 3 - Irregularity Concerns
        /// 4 - Safeguarding Concerns
        /// </summary>
        /// <example>[1, 2, 3]</example>
        public List<int> EsfaInterventionReasons { get; set; }

        /// <summary>
        /// An explanation of the provided ESFA Intervention Reasons. Optional. Max length: 2000 words
        /// </summary>
        /// <example>The ESFA Intervention Reasons explained in detail</example>
        public string EsfaInterventionReasonsExplained { get; set; }

        /// <summary>
        /// An array of RDD or RSC Intervention Reasons. Optional.
        /// 1 - Termination Warning Notice
        /// 2 - RSC Minded To Terminate Notice
        /// 3 - Ofsted Inadequate Rating
        /// </summary>
        public List<int> RddOrRscInterventionReasons { get; set; }

        /// <summary>
        /// An explanation of the provided RDD or RSC Intervention Reasons. Optional. Max length: 2000 words
        /// </summary>
        /// <example>The RDD or RSC Intervention Reasons explained in detail </example>
        public string RddOrRscInterventionReasonsExplained { get; set; }

        /// <summary>
        /// An array of outbound trusts identified for the academy. Optional.
        /// </summary>
        public List<PostProjectsAcademiesTrustsModel> Trusts { get; set; }
    }

    public class PostProjectsAcademiesTrustsModel
    {
        /// <summary>
        /// The Id of the Trust in TRAMS. Mandatory.
        /// </summary>
        [Required]
        public Guid TrustId { get; set; }
    }

    public class PostProjectsTrustsModel
    {
        /// <summary>
        /// The Id of the Trust In TRAMS. Mandatory.
        /// </summary>
        [Required]
        public Guid TrustId { get; set; }
    }
}