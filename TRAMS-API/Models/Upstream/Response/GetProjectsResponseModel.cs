using API.Models.Upstream.Enums;
using System;
using System.Collections.Generic;

namespace API.Models.Upstream.Response
{
    public class GetProjectsResponseModel
    {
        /// <summary>
        /// The ID of the Academy Transfers project
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// The self-generated name of the project
        /// </summary>
        /// <example>AT-1034</example>
        public string ProjectName { get; set; }

        /// <summary>
        /// The full name of the project initiator
        /// </summary>
        /// <example>Joe Bloggs</example>
        public string ProjectInitiatorFullName { get; set; }

        /// <summary>
        /// The unique ID of the project initiator
        /// </summary>
        /// <example>joe.bloggs@email.com</example>
        public string ProjectInitiatorUid { get; set; }

        /// <summary>
        /// The status of the project
        /// </summary>
        public ProjectStatusEnum ProjectStatus { get; set; }

        /// <summary>
        /// The outgoing academies attached to the project
        /// </summary>
        public List<GetProjectsAcademyResponseModel> ProjectAcademies { get; set; }

        /// <summary>
        /// The incoming trusts attached to the project
        /// </summary>
        public List<GetProjectsTrustResponseModel> ProjectTrusts { get; set; }
    }

    public class GetProjectsAcademyResponseModel
    {
        /// <summary>
        /// The ID of the project's academy
        /// </summary>
        public Guid ProjectAcademyId { get; set; }

        /// <summary>
        /// The ID of the parent Academy Transfers project
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// The TRAMS ID of the academy
        /// </summary>
        public Guid AcademyId { get; set; }

        /// <summary>
        /// The name of the academy
        /// </summary>
        /// <example>Imaginary Academy</example>
        public string AcademyName { get; set; }

        /// <summary>
        /// The ESFA intervention reasons selection for this project's academy
        /// </summary>
        public List<EsfaInterventionReasonEnum> EsfaInterventionReasons { get; set; }

        /// <summary>
        /// An explanation of the ESFA intervention reasons
        /// </summary>
        /// <example>The ESFA Intervention Reasons explained</example>
        public string EsfaInterventionReasonsExplained { get; set; }

        /// <summary>
        /// The RDD or RSC Intervention reasons
        /// </summary>
        public List<RddOrRscInterventionReasonEnum> RddOrRscInterventionReasons { get; set; }

        /// <summary>
        /// An exaplanation of the RDD or RSC Intervention reasons
        /// </summary>
        /// <example>The RDD or RSC Intervention reasons explained</example>
        public string RddOrRscInterventionReasonsExplained { get; set; }

        /// <summary>
        /// Information about the proposed incoming trusts for this academy
        /// Data only returned on requesting the project's academy by id
        /// </summary>
        public List<GetAcademyTrustsResponseModel> AcademyTrusts { get; set; }
    }

    public class GetProjectsTrustResponseModel
    {
        /// <summary>
        /// The ID of the project's identified incoming trust
        /// </summary>
        public Guid ProjectTrustId { get; set; }

        /// <summary>
        /// The TRAMS ID of the trust
        /// </summary>
        public Guid TrustId { get; set; }

        /// <summary>
        /// The name of the trust
        /// </summary>
        /// <example>Imaginary Trust</example>
        public string TrustName { get; set; }
    }

    public class GetAcademyTrustsResponseModel
    {
        /// <summary>
        /// The ID of the project's identified incoming trust
        /// </summary>
        public Guid ProjectTrustId { get; set; }

        /// <summary>
        /// The TRAMS ID of the trust
        /// </summary>
        public Guid TrustId { get; set; }

        /// <summary>
        /// The name of the trust
        /// </summary>
        /// <example>Imaginary Trust</example>
        public string TrustName { get; set; }

        public int Priority { get; set; }
    }
}