using API.Models.Upstream.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Upstream.Request
{
    public class PutProjectAcademiesRequestModel
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
        public List<EsfaInterventionReasonEnum> EsfaInterventionReasons { get; set; }

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
        public List<RddOrRscInterventionReasonEnum> RddOrRscInterventionReasons { get; set; }

        /// <summary>
        /// An explanation of the provided RDD or RSC Intervention Reasons. Optional. Max length: 2000 words
        /// </summary>
        /// <example>The RDD or RSC Intervention Reasons explained in detail </example>
        public string RddOrRscInterventionReasonsExplained { get; set; }
    }
}